namespace Web.Src.Model
{
    public class Server
    {
        public string Country { get; init; } = "unknown";
        public string City { get; init; } = "unknown";
        public string Host { get; set; } = "unknown";
        public string Provider { get; init; } = "unknown";
        public double Latitude { get; init; }
        public double Longitude { get; init; }
    }
}
