using System.Net.NetworkInformation;

namespace Web.Src.Service
{
    public sealed class PingService : IPingService
    {
        public async Task<double> CheckPingAsync(string host, int timeout)
        {
            using var ping = new Ping();
            var options = new PingOptions
            {
                Ttl = 128,              //Lifetime TTL packege
                DontFragment = true,
            };
            var buffer = new byte[32];  //Packege size

            try
            {
                var reply = await ping.SendPingAsync(host, timeout, buffer, options);
                return reply.Status == IPStatus.Success ? reply.RoundtripTime : double.MaxValue;
            }
            catch
            {
                return double.MaxValue;
            }
        }
    }
}
