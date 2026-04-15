namespace MatrixGateway.Services;

using MatrixGateway.Models;

public interface IMatrixOrchestrator
{
    Task<FinalResponse> ProcessFullFlowAsync(MatrixRequest request);
}