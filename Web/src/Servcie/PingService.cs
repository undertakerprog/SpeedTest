using System.Net.NetworkInformation;

namespace Web.src.Servcie
{
    public class PingService : IPingService
    {
        public async Task<string> CheckPingAsync(string host)
        {
            using var ping = new Ping();
            var options = new PingOptions
            {
                Ttl = 128,   //Lifetime TTL packege
                DontFragment = true,
            };
            var buffer = new byte[32];  //Packege size
            const int timeout = 5000;

            try
            {
                var reply = await ping.SendPingAsync(host, timeout, buffer, options);
                return reply.Status == IPStatus.Success ? $"Ping to {host} successful: {reply.RoundtripTime} ms" 
                    : $"Ping to {host} failed: {reply.Status}";
            }
            catch (Exception ex)
            {
                return $"Error pining {host}: {ex.Message}";
            }
        }
    }
}
