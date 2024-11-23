using Web.src.Model;

namespace Web.src.Servcie
{
    public interface IServerService
    {
        Task<List<Server>> GetServersAsync();
        Task AddServerAsync(Server newServer);
        Task UpdateSereverAsync(List<Server> servers);
    }
}
