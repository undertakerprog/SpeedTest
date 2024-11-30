using Microsoft.AspNetCore.Mvc;
using Web.src.Servcie;
using Web.src.Model;

namespace Web.src.Сontroller
{
    [ApiController]
    [Route("api/[controller]")]
    public class PingController(IServerService serverService, IPingService pingService) : Controller
    {
        [HttpPost("country-ping")]
        public async Task<IActionResult> PostPingCountry([FromBody] Server selectedServer)
        {
            if (selectedServer == null || string.IsNullOrEmpty(selectedServer.Country))
            {
                return BadRequest("Country can't be null or empty");
            }

            var servers = await serverService.GetServersAsync();

            var countryServers = servers.Where(s => s.Country.Equals(selectedServer.Country, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (countryServers == null)
            {
                return NotFound($"Country: {selectedServer.Country} not found");
            }

            if (countryServers.Count > 1)
            {
                if (string.IsNullOrEmpty(selectedServer.Host))
                {
                    return BadRequest($"Multiple servers found for {selectedServer.Country}. Please specify the 'Host' field.");
                }
                var server = countryServers.FirstOrDefault(s => s.Host.Equals(selectedServer.Host, StringComparison.OrdinalIgnoreCase));
                if (server == null)
                {
                    return NotFound($"Server with host: {selectedServer.Host} not found in country {selectedServer.Country}");
                }
                var pingResult = await pingService.CheckPingAsync(server.Host);
                return Ok(new
                {
                    Country = server.Country,
                    City = server.City,
                    Host = server.Host,
                    PingResult = pingResult
                });
            }

            var singleServer = countryServers.First();
            var singlePingResult = await pingService.CheckPingAsync(singleServer.Host);

            return Ok(new
            {
                Country = singleServer.Country,
                City = singleServer.City,
                Host = singleServer.Host,
                PingResult = singlePingResult
            });
        }

        [HttpPost("host-ping")]
        public async Task<IActionResult> PostPingHost([FromBody] string host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                return BadRequest("Host can't be null or empty");
            }
            var pingResult = await pingService.CheckPingAsync(host);

            return Ok(new
            {
                Host = host,
                PingResult = pingResult
            });
        }
    }
}
