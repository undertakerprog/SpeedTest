using Web.src.Model;

namespace Web.src.Servcie
{
    public interface IServerService
    {
        Task<List<Server>> GetServersAsync();
        Task AddServerAsync(string host);
        Task UpdateSereverAsync(List<Server> servers);
        Task DeleteServerAsync(string country);
    }
}
