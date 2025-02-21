using System.Net.Http;
using System.Text.Json;
using Desktop.Model;
using Desktop.Properties;

namespace Desktop.Service
{
    public class DesktopLocationService(HttpClient httpClient)
    {
        private static string? LocationUri => Settings.Default.LocationUri;

        public async Task<Server?> GetBestServerAsync()
        {
            try
            {
                var response = await httpClient.GetAsync(LocationUri + "best-server");
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var serverResponse = JsonSerializer.Deserialize<ServerResponse>(
                    json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return serverResponse?.Server;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<Server>?> GetServersOfCityAsync(string? city = null)
        {
            try
            {
                var url = city != null
                    ? LocationUri + $"servers-city-list?city={city}"
                    : LocationUri + "servers-city-list";

                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var servers = JsonSerializer.Deserialize<List<Server>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return servers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching servers: {ex.Message}");
                return null;
            }
        }
    }
}
