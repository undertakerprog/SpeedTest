using Newtonsoft.Json;
using Web.src.Model;

namespace Web.src.Servcie
{
    public class ServerService : IServerService
    {
        private readonly string _file = "server.json";

        public async Task<List<Server>> GetServersAsync()
        {
            if (!File.Exists(_file)) 
            {
                throw new FileNotFoundException("File server.json not found");
            }

            var jsonData = await File.ReadAllTextAsync(_file);
            var servers = JsonConvert.DeserializeObject<List<Server>>(jsonData);

            return servers ?? new List<Server>();
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
            var (latitude, longitude, country) = await locationService.GetLocationByIPAsync(host);

            var newServer = new Server
            {
                Host = host,
                Latitude = latitude,
                Longitude = longitude,
                Country = country
            };
            
            servers.Add(newServer);

            var jsonData = JsonConvert.SerializeObject(servers);
            await File.WriteAllTextAsync(_file, jsonData);
        }

        public async Task UpdateSereverAsync(List<Server> servers)
        {
            var jsonData = JsonConvert.SerializeObject(servers, Formatting.Indented);
            await File.WriteAllTextAsync(_file, jsonData);
        }

        public async Task DeleteServerAsync (string country)
        {
            var servers = await GetServersAsync();
            var serverToRemove = servers.FirstOrDefault(s => s.Country.Equals(country, StringComparison.OrdinalIgnoreCase));

            if (serverToRemove == null)
            {
                throw new InvalidOperationException($"Server for country: {country} not found");
            }
            servers.Remove(serverToRemove);

            var jsonData = JsonConvert.SerializeObject(servers, Formatting.Indented);
            await File.WriteAllTextAsync(_file, jsonData);
        }
    }
}
