namespace Web.Src.Model.Location;

public class UserLocationResponse
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Query { get; set; }
}