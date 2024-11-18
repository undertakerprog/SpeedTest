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
        public IActionResult GetServerList()
        {
            try
            {
                var servers = _serverService.GetServers();
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
        public IActionResult AddServer([FromBody] Server newServer)
        {
            if (newServer == null || string.IsNullOrEmpty(newServer.Country) || string.IsNullOrEmpty(newServer.Host))
            {
                return BadRequest("Enter correct data");
            }
            try
            {
                _serverService.AddServer(newServer);
                return Ok("Server added succesfully");
            }
            catch (Exception ex) 
            {
                return StatusCode(500, $"Server Error {ex.Message}");
            }
        }
    }
}
