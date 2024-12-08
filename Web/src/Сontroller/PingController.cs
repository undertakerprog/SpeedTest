using Microsoft.AspNetCore.Mvc;
using Web.src.Servcie;
using Web.src.Model;

namespace Web.src.Сontroller
{
    [ApiController]
    [Route("api/[controller]")]
    public class PingController(IServerService serverService, IPingService pingService) : Controller
    {
        [HttpPost("host-ping")]
        public async Task<IActionResult> PostPingHost([FromBody] string host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                return BadRequest("Host can't be null or empty");
            }

            try
            {
                var pingResult = await pingService.CheckPingAsync(host);
                if (pingResult == double.MaxValue)
                {
                    return Ok(new
                    {
                        Host = host,
                        Message = "Ping failed or host is unreachable"
                    });
                }

                return Ok(new
                {
                    Host = host,
                    PingResult = pingResult
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while performing the ping operation: {ex.Message}");
            }
        }
    }
}
