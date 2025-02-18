using Microsoft.AspNetCore.Mvc;
using Web.Src.Model.SpeedTest;
using Web.Src.Service;

namespace Web.Src.Сontroller
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpeedTestController(ISpeedTestService speedTestService) : Controller
    {
        /// <summary>
        /// Retrieves the download speed for a specified host.
        /// </summary>
        /// <remarks>
        /// This method measures and returns the download speed for the given host.
        /// If no host is provided, the service will use a default host for measurement.
        /// </remarks>
        /// <param name="host">The optional hostname or IP address to test the download speed.</param>
        /// <response code="200">Successfully retrieved the download speed result.</response>
        /// <response code="500">An unexpected server error occurred during the speed test.</response>
        [HttpGet("download-speed")]
        public async Task<IActionResult> GetDownloadSpeed([FromQuery] string? host)
        {
            try
            {
                var result = await speedTestService.GetDownloadSpeed(host);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Performs a fast download speed test.
        /// </summary>
        /// <remarks>
        /// This method quickly measures the download speed within a 7-second timeout.
        /// It is designed for fast and approximate speed testing.
        /// </remarks>
        /// <response code="200">Successfully retrieved the fast download speed result in Mbps.</response>
        /// <response code="500">An unexpected server error occurred during the fast speed test.</response>
        [HttpGet("fast-test")]
        public async Task<IActionResult> GetFastDownloadSpeed()
        {
            try
            {
                var speed = await speedTestService.FastDownloadSpeedAsync(TimeSpan.FromSeconds(7));
                return Ok(new DownloadSpeedResponse
                {
                    SpeedMbps = speed
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }
    }
}
