using Microsoft.AspNetCore.Mvc;
using Web.Src.Model.SpeedTest;
using Web.Src.Service;

namespace Web.Src.Сontroller
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpeedTestController(ISpeedTestService speedTestService) : Controller
    {
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
