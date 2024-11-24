using System.Runtime.InteropServices;
using System.Text.Json;

namespace Web.src.Servcie
{
    public class LocationService : ILocationService
    {
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

        public async Task<(double Latitude, double Longtitude, string Country)> GetLocationByIPAsync(string ipAdress)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync($"http://ip-api.com/json/{ipAdress}");
            var jsonDocument = JsonDocument.Parse(response);

            var latitude = jsonDocument.RootElement.GetProperty("lat").GetDouble();
            var longitude = jsonDocument.RootElement.GetProperty("lon").GetDouble();
            var country = jsonDocument.RootElement.GetProperty("country").GetString() ?? "Unknown county";

            return(latitude, longitude, country);
        }
    }
}
