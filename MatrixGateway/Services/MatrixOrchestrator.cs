namespace MatrixGateway.Services;

using System.Text;
using System.Text.Json;
using MatrixGateway.Models;

public class MatrixOrchestrator : IMatrixOrchestrator
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonOptions;

    public MatrixOrchestrator(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<FinalResponse> ProcessFullFlowAsync(MatrixRequest request)
    {
        var client = _httpClientFactory.CreateClient();

        var goRequestContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var goResponseHttp = await client.PostAsync("http://go-api:3000/api/process", goRequestContent);
        
        if (!goResponseHttp.IsSuccessStatusCode)
            throw new Exception("Error al comunicarse con el motor matemático en Go.");

        var goResponseString = await goResponseHttp.Content.ReadAsStringAsync();
        var goData = JsonSerializer.Deserialize<GoResponse>(goResponseString, _jsonOptions);

        var nodePayload = new { rotated = goData!.Rotated };
        var nodeRequestContent = new StringContent(JsonSerializer.Serialize(nodePayload), Encoding.UTF8, "application/json");
        
        var nodeResponseHttp = await client.PostAsync("http://node-api:4000/api/stats/calculate", nodeRequestContent);
        
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