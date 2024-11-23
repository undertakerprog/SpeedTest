using Microsoft.AspNetCore.Mvc;
using Web.src.Servcie;

namespace Web.src.Сontroller
{
    [ApiController]
    [Route("api/[controller]")]
    public class PingController : Controller
    {
        private readonly IServerService _serverService;
        private readonly IPingService _pingService;

        public PingController(IServerService serverService, IPingService pingService)
        {
            _pingService = pingService;
            _serverService = serverService;
        }

        [HttpPost("country-ping")]
        public async Task<IActionResult> GetPingCountry([FromBody] string country)
        {
            if (string.IsNullOrWhiteSpace(country))
            {
                return BadRequest("Country name can't be null or empty");
            }

            var servers = await _serverService.GetServersAsync();
            var server = servers.FirstOrDefault(s => s.Country.Equals(country, StringComparison.OrdinalIgnoreCase));
            if (server == null)
            {
                return NotFound($"Server for country: {country} not found");
            }
            var pingResult = await _pingService.CheckPingAsync(server.Host);

            return Ok(new
            {
                Country = server.Country,
                Host = server.Host,
                PingResult = pingResult
            });
        }

        [HttpPost("host-ping")]
        public async Task<IActionResult> GetPingHost([FromBody] string  host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                return BadRequest("Host can't be null or empty");
            }
            var pingResult = await _pingService.CheckPingAsync(host);
            return Ok(new
            {
                Host = host,
                PingResult = pingResult
            });
        }
    }
}
