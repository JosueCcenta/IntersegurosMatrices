namespace MatrixGateway.Models;

public class FinalResponse
{
    public GoResponse MathResults { get; set; } = new();
    public NodeStats Statistics { get; set; } = new();
}