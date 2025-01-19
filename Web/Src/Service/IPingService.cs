namespace Web.Src.Service
{
    public interface IPingService
    {
        Task<double> CheckPingAsync(string host, int timeout);
    }
}
