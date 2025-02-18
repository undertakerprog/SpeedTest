using Microsoft.AspNetCore.Mvc;
using Web.Src.Model.Ping;
using Web.Src.Service;

namespace Web.Src.Сontroller
{
    [ApiController]
    [Route("api/[controller]")]
    public class PingController(IPingService pingService) : Controller
    {
        /// <summary>
        /// Performs a ping operation to check the reachability of a specified host.
        /// </summary>
        /// <remarks>
        /// This method sends a ping request to the specified host and returns the result.
        /// If the host is unreachable or the ping operation fails, an error response will be returned.
        /// The timeout for the ping operation is set to 5 seconds (5000 ms).
        /// </remarks>
        /// <param name="host">The hostname or IP address of the target host.</param>
        /// <response code="200">Ping operation successful. Returns either the ping result or an error message.</response>
        /// <response code="400">Invalid input. Host parameter is null or empty.</response>
        /// <response code="500">An unexpected error occurred during the ping operation.</response>
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
