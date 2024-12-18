namespace Web.Src.Model.Location;

public class UserLocationResponse
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string? Country { get; init; }
    public string? City { get; init; }
    public string? Query { get; init; }
}