using System.Text.Json;
using Web.src.Model;

namespace Web.src.Servcie
{
    public class LocationService : ILocationService
    {
        private readonly string _filePath = "server.json";

        public async Task<(double Latitude, double Longitude, string Country, string City, string Query)> GetUserLocationAsync()
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync("http://ip-api.com/json/");
            var jsonDocument = JsonDocument.Parse(response);

            var latitude = jsonDocument.RootElement.GetProperty("lat").GetDouble();
            var longitude = jsonDocument.RootElement.GetProperty("lon").GetDouble();
            var country = jsonDocument.RootElement.GetProperty("country").GetString() ?? "Unknown county";
            var query = jsonDocument.RootElement.GetProperty("query").GetString() ?? "Unknown city";
            var city = jsonDocument.RootElement.GetProperty("city").GetString() ?? "Unknown query";

            return (latitude, longitude, country, city, query);
        }

        public async Task<(double Latitude, double Longtitude, string Country, string City)> GetLocationByIPAsync(string ipAdress)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync($"http://ip-api.com/json/{ipAdress}");
            var jsonDocument = JsonDocument.Parse(response);

            var latitude = jsonDocument.RootElement.GetProperty("lat").GetDouble();
            var longitude = jsonDocument.RootElement.GetProperty("lon").GetDouble();
            var country = jsonDocument.RootElement.GetProperty("country").GetString() ?? "Unknown county";
            var city = jsonDocument.RootElement.GetProperty("city").GetString() ?? "Unknown county";

            return (latitude, longitude, city, country);
        }

        public async Task<Server?> GetClosestServerAsync()
        {
            var (userLat, userLon, _, _, _) = await GetUserLocationAsync();

            var fileContent = await File.ReadAllTextAsync(_filePath);
            var servers = JsonSerializer.Deserialize<List<Server>>(fileContent) ?? new List<Server>();

            if (!servers.Any())
            {
                throw new InvalidOperationException("No server availiable in the list");
            }

            Server? closestServer = null;
            double closestDistanse = double.MaxValue;

            foreach (var server in servers)
            {
                double distanse = CalculateDistanse(userLat, userLon, server.Latitude, server.Longitude);

                if (distanse < closestDistanse)
                {
                    closestDistanse = distanse;
                    closestServer = server;
                }
            }
            return closestServer;
        }

        private double CalculateDistanse(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;
            double dLat = DegreesToRadians(lat2 - lat1);
            double dLon = DegreesToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + 
                Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
