namespace Web.src.Model
{
    public class Server
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Host { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Server()
        {
            Country = "unknown";
            City = "unknown";
            Host = "unknown";
        }
    }
}
