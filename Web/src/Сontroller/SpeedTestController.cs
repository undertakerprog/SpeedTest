using Microsoft.AspNetCore.Mvc;
using Web.src.Servcie;

namespace Web.src.controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpeedTestController : Controller
    {
        private readonly ISpeedTestService _speedTestService;

        public SpeedTestController(ISpeedTestService speedTestService)
        {
            _speedTestService = speedTestService;
        }

        [HttpGet("interface")]
        public IActionResult GetInterface()
        {
            var result = _speedTestService.GetInterface();
            return Ok(result);
        }


    }
}
