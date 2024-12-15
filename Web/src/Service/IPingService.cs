namespace Web.src.Service
{
    public interface IPingService
    {
        Task<double> CheckPingAsync(string host);
    }
}
