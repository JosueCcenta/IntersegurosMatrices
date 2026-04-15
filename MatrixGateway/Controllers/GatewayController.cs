namespace MatrixGateway.Controllers;

using Microsoft.AspNetCore.Mvc;
using MatrixGateway.Models;
using MatrixGateway.Services;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/gateway")]
public class GatewayController : ControllerBase
{
    private readonly IMatrixOrchestrator _orchestrator;

    public GatewayController(IMatrixOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }
    
    [Authorize]
    [HttpPost("analyze")]
    public async Task<IActionResult> AnalyzeMatrix([FromBody] MatrixRequest request)
    {
        try
        {
            if (request.Data == null || request.Data.Length == 0)
                return BadRequest("La matriz no puede estar vacía.");

            var result = await _orchestrator.ProcessFullFlowAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { ex.Message });
        }
    }
}