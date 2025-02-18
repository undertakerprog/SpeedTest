using Microsoft.AspNetCore.Mvc;
using Web.Src.Service;
using Web.Src.Model;

namespace Web.Src.Сontroller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServerController(IServerService serverService) : Controller
    {
        /// <summary>
        /// Retrieves the list of all servers.
        /// </summary>
        /// <remarks>
        /// This method fetches and returns a list of all available servers from the service.
        /// If no servers are found, an empty list will be returned.
        /// </remarks>
        /// <response code="200">Successfully retrieved the list of servers.</response>
        /// <response code="404">Server list file not found.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [HttpGet("list")]
        public async Task<IActionResult> GetServerList()
        {
            try
            {
                var servers = await serverService.GetServersAsync();
                return Ok(servers);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds a new server to the server list.
        /// </summary>
        /// <remarks>
        /// This method adds a new server based on the provided host. 
        /// The host must be a valid hostname or IP address.
        /// After adding, it retrieves and returns the updated server details.
        /// </remarks>
        /// <param name="host">The hostname or IP address of the server to add.</param>
        /// <response code="200">Server added successfully with details.</response>
        /// <response code="400">Invalid or missing host parameter.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [HttpPost("add")]
        public async Task<IActionResult>AddServer([FromBody] string host)
        {
            if (string.IsNullOrEmpty(host))
            {
                return BadRequest("Enter correct data");
            }
            try
            {
                await serverService.AddServerAsync(host);
                var servers = await serverService.GetServersAsync();
                var server = servers.FirstOrDefault(s =>
                    s.Host.Equals(host, StringComparison.OrdinalIgnoreCase)) ?? new Server();
                return Ok($"Server added successfully({server.Host}).\nCountry: {server.Country}\nCity: {server.City} \n");
            }
            catch (Exception ex) 
            {
                return StatusCode(500, $"Server Error {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the host of an existing server.
        /// </summary>
        /// <remarks>
        /// This method updates the host of a server identified by the old host value.
        /// Both old and new hosts must be provided and cannot be null or empty.
        /// If the server is not found, a 404 response will be returned.
        /// </remarks>
        /// <param name="request">The request containing the old and new host values.</param>
        /// <response code="200">Old host successfully updated to the new host.</response>
        /// <response code="400">Invalid or missing request data.</response>
        /// <response code="404">Old host not found in the server list.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateHost([FromBody] UpdateHostRequest? request)
        {
            if (request == null || string.IsNullOrEmpty(request.OldHost) || 
                string.IsNullOrEmpty(request.NewHost))
            {
                return BadRequest("Enter correct data");
            }
            try
            {
                var servers = await serverService.GetServersAsync();
                var server = servers.FirstOrDefault(s => 
                    s.Host.Equals(request.OldHost, StringComparison.OrdinalIgnoreCase));
                if (server == null)
                {
                    return NotFound($"Old host: {request.OldHost} not found");
                }
                var oldHost = server.Host;
                server.Host = request.NewHost;
                await serverService.UpdateServerAsync(servers);
                return Ok($"Old host {request.OldHost} update form " +
                          $"{oldHost} to {request.NewHost}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a server based on city and optional host.
        /// </summary>
        /// <remarks>
        /// This method deletes a server matching the specified city and optionally the host.
        /// If no host is provided, all servers in the specified city will be deleted.
        /// </remarks>
        /// <param name="city">The city of the server to delete.</param>
        /// <param name="host">The optional host of the server to delete.</param>
        /// <response code="200">Server successfully deleted.</response>
        /// <response code="400">City parameter is null or empty.</response>
        /// <response code="404">No server found for the specified city or host.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [HttpDelete("delete-server")]
        public async Task<IActionResult> DeleteServer([FromQuery] string city, [FromQuery] string? host = null)
        {
            if (string.IsNullOrEmpty(city))
            {
                return BadRequest("OldHost can't be null or empty");
            }

            try
            {
                await serverService.DeleteServerAsync(city, host);
                return Ok($"Server successfully deleted for city: {city} with host {host}");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes all servers for a specified country.
        /// </summary>
        /// <remarks>
        /// This method deletes all servers belonging to the specified country.
        /// If no servers are found for the given country, a 404 response will be returned.
        /// </remarks>
        /// <param name="country">The country for which servers should be deleted.</param>
        /// <response code="200">All servers for the specified country successfully deleted.</response>
        /// <response code="400">Country parameter is null or empty.</response>
        /// <response code="404">No servers found for the specified country.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [HttpDelete("delete-all")]
        public async Task<IActionResult> DeleteAllServer([FromBody] string country)
        {
            if (string.IsNullOrEmpty(country))
            {
                return BadRequest("OldHost can't be null or empty");
            }
            try
            {
                await serverService.DeleteAllServerAsync(country);
                return Ok($"All servers for country: '{country}' successfully deleted");
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }
    }
}
