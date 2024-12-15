using Microsoft.AspNetCore.Mvc;
using Web.src.Servcie;

namespace Web.src.Сontroller
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpeedTestController(ISpeedTestService speedTestService) : Controller
    {
        [HttpGet("interface")]
        public IActionResult GetInterface()
        {
            var result = speedTestService.GetInterface();
            return Ok(result);
        }

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

        [HttpGet("fast-test")]
        public async Task<IActionResult> GetFastDownloadSpeed()
        {
            try
            {
                var speed = await speedTestService.FastDownloadSpeedAsync(TimeSpan.FromSeconds(7));
                return Ok(new
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
