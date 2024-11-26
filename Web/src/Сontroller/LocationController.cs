using Microsoft.AspNetCore.Mvc;
using Web.src.Servcie;

namespace Web.src.Сontroller
{
    [ApiController]
    [Route("api/location")]
    public class LocationController : Controller
    {
        private readonly ILocationService _locationService;
        public LocationController(ILocationService locationService) 
        {
            _locationService = locationService;
        }

        [HttpGet("my-location")]
        public async Task<IActionResult> GetUserLocation()
        {
            try
            {
                var location = await _locationService.GetUserLocationAsync();
                return Ok(new
                {
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    Country = location.Country,
                    City = location.City,
                    Query = location.Query,
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
                var closestServer = await _locationService.GetClosestServerAsync();

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
    }
}
