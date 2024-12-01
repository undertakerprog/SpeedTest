using System.Text.Json;
using Web.src.Model;

namespace Web.src.Servcie
{
    public class LocationService : ILocationService
    {
        private const string FilePath = "server.json";

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
            var (userLat, userLon, _, userCity, _) = await GetUserLocationAsync();
            
            var fileContent = await File.ReadAllTextAsync(FilePath);
            var servers = JsonSerializer.Deserialize<List<Server>>(fileContent) ?? [];

            if (!servers.Any())
            {
                throw new InvalidOperationException("No server available in the list");
            }

            var serversInSameCity = servers.Where(s => s.City.Equals(userCity, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return FindClosestServer(userLat, userLon, serversInSameCity.Any() ? serversInSameCity : servers);
        }

        private static Server? FindClosestServer(double userLat, double userLon, List<Server> servers)
        {
            Server? closestServer = null;
            var closestDistance = double.MaxValue;

            foreach (var server in servers)
            {
                var distance = CalculateDistance(userLat, userLon, server.Latitude, server.Longitude);

                if (!(distance < closestDistance)) continue;
                closestDistance = distance;
                closestServer = server;
            }
            return closestServer;
        }

        private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double r = 6371;
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + 
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return r * c;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
