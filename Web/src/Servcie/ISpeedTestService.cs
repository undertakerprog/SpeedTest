using Web.src.Model;

namespace Web.src.Servcie
{
    public interface ISpeedTestService
    {
        string GetInterface();
        Task<double> FastDownloadSpeedAsync(TimeSpan duration);
        Task<DownloadSpeed> GetDownloadSpeed();
    }
}