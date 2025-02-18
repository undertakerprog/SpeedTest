using Microsoft.AspNetCore.Mvc;
using Web.Src.Model;
using Web.Src.Model.Location;
using Web.Src.Service;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Web.Src.Сontroller
{
    [ApiController]
    [Route("api/location")]
    public class LocationController(ILocationService locationService, IRedisCacheService cacheService) : Controller
    {
        /// <summary>
        /// Fetches the current user's location.
        /// </summary>
        /// <remarks>
        /// This method retrieves the geographical coordinates, country, and city of the current user.
        /// If the user's location cannot be determined, an error will be returned.
        /// </remarks>
        /// <response code="200">Successfully retrieved user location.</response>
        /// <response code="400">Invalid request parameters.</response>
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

        /// <summary>
        /// Retrieves geolocation data for a given host IP address.
        /// </summary>
        /// <remarks>
        /// This method allows you to obtain detailed geographical information about a specific host.
        /// The input can be either an IP address or a hostname. The response includes latitude, longitude,
        /// country, and city of the specified host.
        /// </remarks>
        /// <param name="host">The IP address or hostname of the target host.</param>
        /// <response code="200">Successfully retrieved host location.</response>
        /// <response code="400">Invalid request parameters.</response>
        [HttpGet("host-location")]
        public async Task<IActionResult> GetHostLocation([FromQuery] string host)
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

        /// <summary>
        /// Retrieves the closest server to the user's location.
        /// </summary>
        /// <remarks>
        /// This method returns information about the server that is geographically closest to the user.
        /// If no servers are available, a 404 response will be returned.
        /// </remarks>
        /// <response code="200">Successfully retrieved closest server.</response>
        /// <response code="404">No servers found.</response>
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

        /// <summary>
        /// Retrieves the best server based on performance metrics.
        /// </summary>
        /// <remarks>
        /// This method returns information about the server with the best quality of service.
        /// If no servers are available, a 404 response will be returned.
        /// </remarks>
        /// <response code="200">Successfully retrieved best server.</response>
        /// <response code="404">No servers found.</response>
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

        /// <summary>
        /// Retrieves a list of servers located in the specified city.
        /// </summary>
        /// <remarks>
        /// This method returns a list of servers that are located in the specified city.
        /// If no city is provided, the user's city will be used by default.
        /// If no servers are found for the specified city, a 404 response will be returned.
        /// </remarks>
        /// <param name="city">The city for which to retrieve the server list. If not provided, defaults to the user's city.</param>
        /// <response code="200">Successfully retrieved server list.</response>
        /// <response code="404">No servers found for the specified city.</response>
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

                var cacheKey = $"server-city:{city.ToLower()}";

                var cacheData = await cacheService.GetCachedValueAsync(cacheKey);
                if (!string.IsNullOrEmpty(cacheData))
                {
                    var cachedServers = JsonSerializer.Deserialize<List<Server>>(cacheData);
                    return Ok(cachedServers);
                }

                var servers = await locationService.GetServersByCityAsync(city);

                if (!servers.Any())
                {
                    return NotFound($"Servers with city: {city} not found");
                }

                await cacheService.SetCachedValueAsync(cacheKey, JsonSerializer.Serialize(servers),
                    TimeSpan.FromMinutes(30));

                var memoryInfo = await cacheService.GetRedisInfoAsync("memory");
                Console.WriteLine($"Redis memory usage: {memoryInfo["used_memory_human"]}");
                Console.WriteLine($"City '{city}' has been cached with key: '{cacheKey}'");

                return Ok(servers);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }
    }
}
