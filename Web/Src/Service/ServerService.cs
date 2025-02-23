using Infrastructure;
using Newtonsoft.Json;
using Web.Src.Model;

namespace Web.Src.Service
{
    public class ServerService(IFileReader fileReader) : IServerService
    {
        private const string File = "..//Web//server.json";

        public async Task<List<Server>> GetServersAsync()
        {
            if (!await fileReader.ExistsAsync(File)) 
            {
                throw new FileNotFoundException("File server.json not found");
            }

            var jsonData = await fileReader.ReadAllTextAsync(File);

            try
            {
                var servers = JsonConvert.DeserializeObject<List<Server>>(jsonData);
                return servers ?? [];
            }
            catch (JsonSerializationException)
            {
                return [];
            }
        }

        public async Task AddServerAsync(string host)
        {
            var servers = await GetServersAsync();
            var existingServer = servers.FirstOrDefault(s => s.Host == host);
            if (existingServer != null)
            {
                throw new Exception($"Server with host: {host} already exists");
            }

            var locationService = new LocationService(new HttpClient());
            var (latitude, longitude, city, country) = 
                await locationService.GetLocationByIpAsync(host);

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
            await fileReader.WriteAllTextAsync(File, jsonData);
        }

        public async Task DeleteServerAsync(string? city = null, string? host = null)
        {
            var servers = await GetServersAsync();

            if (string.IsNullOrEmpty(city) && string.IsNullOrEmpty(host))
            {
                throw new ArgumentException("At least one parameter (city or host) must be provided.");
            }

            List<Server> matchingServers;

            if (!string.IsNullOrEmpty(city))
            {
                matchingServers = servers.Where(s => s.City.Equals(city, StringComparison.OrdinalIgnoreCase)).ToList();

                switch (matchingServers.Count)
                {
                    case 0:
                        throw new InvalidOperationException($"No servers found for city: {city}");
                    case > 1 when string.IsNullOrEmpty(host):
                        throw new ArgumentException($"Multiple servers found in city: {city}. Please specify a host.");
                }
            }
            else
            {
                matchingServers = servers.Where(s => s.Host.Equals(host, StringComparison.OrdinalIgnoreCase)).ToList();

                if (matchingServers.Count == 0)
                {
                    throw new InvalidOperationException($"No server found with host: {host}");
                }
            }

            var serverToRemove = matchingServers.FirstOrDefault(s =>
                string.IsNullOrEmpty(host) || s.Host.Equals(host, StringComparison.OrdinalIgnoreCase));

            if (serverToRemove == null)
            {
                throw new InvalidOperationException($"Server with host: {host} not found in city: {city}");
            }

            servers.Remove(serverToRemove);
            var jsonData = JsonConvert.SerializeObject(servers, Formatting.Indented);

            await fileReader.WriteAllTextAsync(File, jsonData);
        }


        public async Task DeleteAllServerAsync (string country)
        {
            var servers = await GetServersAsync();
            var updatedServer = servers.Where(s => 
                !s.Country.Equals(country, StringComparison.OrdinalIgnoreCase)).ToList();

            if (updatedServer.Count == servers.Count)
            {
                throw new InvalidOperationException($"Server for country: {country} not found");
            }

            var jsonData = JsonConvert.SerializeObject(updatedServer, Formatting.Indented);
            await fileReader.WriteAllTextAsync(File, jsonData);
        }
    }
}
