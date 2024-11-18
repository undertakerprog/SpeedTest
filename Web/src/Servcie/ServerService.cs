using Newtonsoft.Json;
using Web.src.Model;

namespace Web.src.Servcie
{
    public class ServerService : IServerService
    {
        private readonly string _file = "server.json";

        public List<Server> GetServers()
        {
            if (!File.Exists(_file)) 
            {
                throw new FileNotFoundException("File server.json not found");
            }

            var jsonData = File.ReadAllText(_file);
            var servers = JsonConvert.DeserializeObject<List<Server>>(jsonData);

            return servers ?? new List<Server>();
        }

        public void AddServer(Server newServer)
        {
            var servers = GetServers();
            servers.Add(newServer);

            var jsonData = JsonConvert.SerializeObject(servers);
            File.WriteAllText(_file, jsonData);
        }
    }
}
