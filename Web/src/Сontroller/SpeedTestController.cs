using Microsoft.AspNetCore.Mvc;
using Web.src.Servcie;

namespace Web.src.controller
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
    }
}
