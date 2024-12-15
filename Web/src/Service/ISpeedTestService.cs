using Web.src.Model;

namespace Web.src.Service
{
    public interface ISpeedTestService
    {
        string GetInterface();
        Task<double> FastDownloadSpeedAsync(TimeSpan duration);
        Task<DownloadSpeed> GetDownloadSpeed(string? host = null);
    }
}