using Microsoft.AspNetCore.Mvc;
using Web.src.Servcie;
using Web.src.Model;

namespace Web.src.Сontroller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServerController : Controller
    {
        private readonly IServerService _serverService;

        public ServerController(IServerService serverService)
        {
            _serverService = serverService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetServerList()
        {
            try
            {
                var servers = await _serverService.GetServersAsync();
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

        [HttpPost("add")]
        public async Task<IActionResult>AddServer([FromBody] string host)
        {
            if (string.IsNullOrEmpty(host))
            {
                return BadRequest("Enter correct data");
            }
            try
            {
                await _serverService.AddServerAsync(host);
                var servers = await _serverService.GetServersAsync();
                var server = servers.FirstOrDefault(s => s.Host.Equals(host, StringComparison.OrdinalIgnoreCase)) ?? new Server();
                return Ok($"Server added succesfully({server.Host}).\nCountry: {server.Country}\nCity: {server.City} \n");
            }
            catch (Exception ex) 
            {
                return StatusCode(500, $"Server Error {ex.Message}");
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateHost([FromBody] UpdateHostRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Country) || string.IsNullOrEmpty(request.NewHost))
            {
                return BadRequest("Enter correct data");
            }
            try
            {
                var servers = await _serverService.GetServersAsync();
                var server = servers.FirstOrDefault(s => s.Country.Equals(request.Country, StringComparison.OrdinalIgnoreCase));
                if (server == null)
                {
                    return NotFound($"Server for country: {request.Country} not found");
                }
                var oldHost = server.Host;
                server.Host = request.NewHost;
                await _serverService.UpdateSereverAsync(servers);
                return Ok($"Host for country {request.Country} update form {oldHost} to {request.NewHost}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

            [HttpDelete("delete-server")]
            public async Task<IActionResult> DeleteServer([FromQuery] string country, [FromQuery] string? host = null)
            {
                if (string.IsNullOrEmpty(country))
                {
                    return BadRequest("Country can't be null or empty");
                }
                try
                {
                    await _serverService.DeleteServerAsync(country, host);
                    return Ok($"Server successfully deleted for country: {country} with host {host}");
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

        [HttpDelete("delete-all")]
        public async Task<IActionResult> DeleteAllServer([FromBody] string country)
        {
            if (string.IsNullOrEmpty(country))
            {
                return BadRequest("Country can't be null or empty");
            }
            try
            {
                await _serverService.DeleteAllServerAsync(country);
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
