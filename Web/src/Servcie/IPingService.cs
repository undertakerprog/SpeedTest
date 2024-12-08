namespace Web.src.Servcie
{
    public interface IPingService
    {
        Task<double> CheckPingAsync(string host);
    }
}
