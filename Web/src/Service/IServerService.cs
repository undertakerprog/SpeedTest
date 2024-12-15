using Web.Src.Model;

namespace Web.Src.Service
{
    public interface IServerService
    {
        Task<List<Server>> GetServersAsync();
        Task AddServerAsync(string host);
        Task UpdateServerAsync(List<Server> servers);
        Task DeleteServerAsync(string country, string? host = null);
        Task DeleteAllServerAsync(string country);
    }
}
