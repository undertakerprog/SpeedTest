using Web.Src.Model;

namespace Web.Src.Service
{
    public interface ISpeedTestService
    {
        Task<double> FastDownloadSpeedAsync(TimeSpan duration);
        Task<DownloadSpeed> GetDownloadSpeed(string? host = null);
    }
}