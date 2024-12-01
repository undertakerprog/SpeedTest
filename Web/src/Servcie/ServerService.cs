using Newtonsoft.Json;
using Web.src.Model;

namespace Web.src.Servcie
{
    public class ServerService : IServerService
    {
        private const string File = "server.json";

        public async Task<List<Server>> GetServersAsync()
        {
            if (!System.IO.File.Exists(File)) 
            {
                throw new FileNotFoundException("File server.json not found");
            }

            var jsonData = await System.IO.File.ReadAllTextAsync(File);
            var servers = JsonConvert.DeserializeObject<List<Server>>(jsonData);

            return servers ?? [];
        }

        public async Task AddServerAsync(string host)
        {
            var servers = await GetServersAsync();
            var existingServer = servers.FirstOrDefault(s => s.Host == host);
            if (existingServer != null)
            {
                throw new Exception($"Server with host: {host} already exists");
            }
            var locationService = new LocationService();
            var (latitude, longitude, city, country) = await locationService.GetLocationByIPAsync(host);

            var newServer = new Server
            {
                Host = host,
                Latitude = latitude,
                Longitude = longitude,
                Country = country,
                City = city
            };
            
            servers.Add(newServer);

            var jsonData = JsonConvert.SerializeObject(servers);
            await System.IO.File.WriteAllTextAsync(File, jsonData);
        }

        public async Task UpdateServerAsync(List<Server> servers)
        {
            var jsonData = JsonConvert.SerializeObject(servers, Formatting.Indented);
            await System.IO.File.WriteAllTextAsync(File, jsonData);
        }

        public async Task DeleteServerAsync(string city, string? host = null)
        {
            var servers = await GetServersAsync();
            var cityServer = servers.Where(s => s.City.Equals(city, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!cityServer.Any())
            {
                throw new InvalidOperationException($"No server for city: {city}");
            }

            Server? serverToRemove;

            if (cityServer.Count() > 1)
            {
                if (string.IsNullOrEmpty(host))
                {
                    throw new ArgumentException($"Multiple servers found for city: {city}. Please specify the host.");
                }
                serverToRemove = cityServer.FirstOrDefault(s => s.Host.Equals(host, StringComparison.OrdinalIgnoreCase));
                if (serverToRemove == null)
                {
                    throw new InvalidOperationException($"Server with host: {host} not found in city: {city}");
                }
            }
            else
            {
                serverToRemove = cityServer.First();
            }
            servers.Remove(serverToRemove);

            var jsonData = JsonConvert.SerializeObject(servers, Formatting.Indented);
            await System.IO.File.WriteAllTextAsync(File, jsonData);
        }

        public async Task DeleteAllServerAsync (string country)
        {
            var servers = await GetServersAsync();
            var updatedServer = servers.Where(s => !s.Country.Equals(country, StringComparison.OrdinalIgnoreCase)).ToList();

            if (updatedServer.Count == servers.Count)
            {
                throw new InvalidOperationException($"Server for country: {country} not found");
            }

            var jsonData = JsonConvert.SerializeObject(updatedServer, Formatting.Indented);
            await System.IO.File.WriteAllTextAsync(File, jsonData);
        }
    }
}
