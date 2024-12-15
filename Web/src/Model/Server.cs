namespace Web.Src.Model
{
    public class Server
    {
        public string Country { get; set; } = "unknown";
        public string City { get; set; } = "unknown";
        public string Host { get; set; } = "unknown";
        public string Provider { get; set; } = "unknown";
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
