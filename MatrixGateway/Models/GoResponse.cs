namespace MatrixGateway.Models;

public class GoResponse
{
    public double[][] Original { get; set; } = [];
    public double[][] Rotated { get; set; } = [];
    public double[][] Q_Matrix { get; set; } = [];
    public double[][] R_Matrix { get; set; } = [];
}