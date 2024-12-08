using Microsoft.AspNetCore.Mvc;
using Web.src.Servcie;

namespace Web.src.Сontroller
{
    [ApiController]
    [Route("api/location")]
    public class LocationController(ILocationService locationService) : Controller
    {
        [HttpGet("my-location")]
        public async Task<IActionResult> GetUserLocation()
        {
            try
            {
                var location = await locationService.GetUserLocationAsync();
                return Ok(new
                {
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    Country = location.Country,
                    City = location.City,
                    Query = location.Query
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving location: {ex.Message}");
            }
        }

        [HttpPost("host-location")]
        public async Task<IActionResult> PostHostLocation([FromBody] string host)
        {
            try
            {
                var location = await locationService.GetLocationByIPAsync(host);
                return Ok(new
                {
                    Latitude = location.Latitude,
                    Longitude = location.Longtitude,
                    Country = location.Country,
                    City = location.City,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving location: {ex.Message}");
            }
        }

        [HttpGet("closest")]
        public async Task<IActionResult> GetClosestServer()
        {
            try
            {
                var closestServer = await locationService.GetClosestServerAsync();

                if (closestServer == null)
                {
                    return NotFound("No servers found");
                }
                return Ok(new
                {
                    Message = "Closest server found",
                    Server = closestServer
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

        [HttpGet("best-server")]
        public async Task<IActionResult> GetBestServer()
        {
            try
            {
                var bestServer = await locationService.GetBestServerAsync();
                if (bestServer == null)
                {
                    return NotFound("No servers found");
                }
                return Ok(new
                {
                    Message = "Closest server found",
                    Server = bestServer
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }
    }
}
