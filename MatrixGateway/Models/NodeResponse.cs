namespace MatrixGateway.Models;

public class NodeStats
{
    public double Max { get; set; }
    public double Min { get; set; }
    public double Average { get; set; }
    public double Sum { get; set; }
    public bool IsDiagonal { get; set; }
}

public class NodeResponse
{
    public bool Success { get; set; }
    public NodeStats Stats { get; set; } = new();
}