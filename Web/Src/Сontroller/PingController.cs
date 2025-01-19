using Microsoft.AspNetCore.Mvc;
using Web.Src.Model.Ping;
using Web.Src.Service;

namespace Web.Src.Сontroller
{
    [ApiController]
    [Route("api/[controller]")]
    public class PingController(IPingService pingService) : Controller
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
                var pingResult = await pingService.CheckPingAsync(host, 5000);
                if (pingResult == double.MaxValue)
                {
                    return Ok(new PingErrorResponse
                    {
                        Host = host,
                        Message = "Ping failed or host is unreachable"
                    });
                }
                
                return Ok(new PingResponse
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
