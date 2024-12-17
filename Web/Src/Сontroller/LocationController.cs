using Microsoft.AspNetCore.Mvc;
using Web.Src.Model.Location;
using Web.Src.Service;

namespace Web.Src.Сontroller
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
                var response = new UserLocationResponse
                {
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    Country = location.Country,
                    City = location.City,
                    Query = location.Query
                };
                return Ok(response);
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
                var location = await locationService.GetLocationByIpAsync(host);
                var response = new HostLocationResponse
                {
                    Latitude = location.Latitude,
                    Longitude = location.Longtitude,
                    Country = location.Country,
                    City = location.City
                };

                return Ok(response);
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
                var response = new ServerResponse
                {
                    Message = "Closest server found",
                    Server = closestServer
                };
                return Ok(response);
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
                var response = new ServerResponse
                {
                    Message = "Best server found",
                    Server = bestServer
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

        [HttpGet("servers-city-list")]
        public async Task<IActionResult> GetServersOfCity([FromQuery] string? city)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(city))
                {
                    var (_, _, _, userCity, _) = await locationService.GetUserLocationAsync();
                    city = userCity;
                }

                var servers = await locationService.GetServersByCityAsync(city);

                if (!servers.Any())
                {
                    return NotFound($"Servers with city: {city} not found");
                }

                return Ok(servers);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        } 
    }
}
