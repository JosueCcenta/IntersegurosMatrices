namespace MatrixGateway.Services;

using System.Text;
using System.Text.Json;
using MatrixGateway.Models;

public class MatrixOrchestrator : IMatrixOrchestrator
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly JsonSerializerOptions _jsonOptions;

    public MatrixOrchestrator(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<FinalResponse> ProcessFullFlowAsync(MatrixRequest request)
    {
        var client = _httpClientFactory.CreateClient();

        string goApiUrl = _config["GO_API_URL"];
        string nodeApiUrl = _config["NODE_API_URL"];

        var goRequestContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var goResponseHttp = await client.PostAsync($"{goApiUrl}/api/process", goRequestContent);
        
        if (!goResponseHttp.IsSuccessStatusCode)
            throw new Exception("Error al comunicarse con el motor matemático en Go.");

        var goResponseString = await goResponseHttp.Content.ReadAsStringAsync();
        var goData = JsonSerializer.Deserialize<GoResponse>(goResponseString, _jsonOptions);

        var nodePayload = new { rotated = goData!.Rotated };
        var nodeRequestContent = new StringContent(JsonSerializer.Serialize(nodePayload), Encoding.UTF8, "application/json");
        
        var nodeResponseHttp = await client.PostAsync($"{nodeApiUrl}/api/stats/calculate", nodeRequestContent);
        
        if (!nodeResponseHttp.IsSuccessStatusCode)
            throw new Exception("Error al comunicarse con el procesador de estadísticas en Node.js.");

        var nodeResponseString = await nodeResponseHttp.Content.ReadAsStringAsync();
        var nodeData = JsonSerializer.Deserialize<NodeResponse>(nodeResponseString, _jsonOptions);

        return new FinalResponse
        {
            MathResults = goData,
            Statistics = nodeData!.Stats
        };
    }
}