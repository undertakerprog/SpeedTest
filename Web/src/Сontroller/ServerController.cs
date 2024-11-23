using Microsoft.AspNetCore.Mvc;
using Web.src.Servcie;
using Web.src.Model;
using System.Net.NetworkInformation;

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
        public async Task<IActionResult>AddServer([FromBody] Server newServer)
        {
            if (newServer == null || string.IsNullOrEmpty(newServer.Country) || string.IsNullOrEmpty(newServer.Host))
            {
                return BadRequest("Enter correct data");
            }
            try
            {
                await _serverService.AddServerAsync(newServer);
                return Ok("Server added succesfully");
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
                    return NotFound("$\"Server for country: {country} not found\"");
                }
                var oldHost = server.Host;
                server.Host = request.NewHost;
                await _serverService.SaveSereverAsync(servers);
                return Ok($"Host for country {request.Country} update form {oldHost} to {request.NewHost}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }
    }
}
