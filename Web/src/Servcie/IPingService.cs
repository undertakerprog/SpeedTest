namespace Web.src.Servcie
{
    public interface IPingService
    {
        Task<string> CheckPingAsync(string host);
    }
}
