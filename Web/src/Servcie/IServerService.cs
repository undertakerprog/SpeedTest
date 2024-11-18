using Web.src.Model;

namespace Web.src.Servcie
{
    public interface IServerService
    {
        List<Server> GetServers();
        void AddServer(Server newServer);
    }
}
