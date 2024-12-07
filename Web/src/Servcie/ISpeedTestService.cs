using Web.src.Model;

namespace Web.src.Servcie
{
    public interface ISpeedTestService
    {
        string GetInterface();
        Task<double> MeasureDownloadSpeedAsync(TimeSpan duration);
    }
}