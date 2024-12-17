using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Src.Model;
using Web.Src.Service;
using Web.Src.Сontroller;

namespace WebTest.Src.ControllerTest
{
    [TestClass]
    public class ServerControllerTest
    {
        private Mock<IServerService>? _mockServerService;
        private ServerController? _serverController;

        [TestInitialize]
        public void Setup()
        {
            _mockServerService = new Mock<IServerService>();
            _serverController = new ServerController(_mockServerService.Object);
        }

        [TestMethod]
        public async Task GetServerList_ServersFound_ReturnsOkWithServers()
        {
            var servers = new List<Server>
            {
                new () { Host = "server1.com", City = "New York", Provider = "Provider A" },
                new () { Host = "server2.com", City = "Los Angeles", Provider = "Provider B" }
            };

            _mockServerService!.Setup(service => service.GetServersAsync())
                .ReturnsAsync(servers);

            var result = await _serverController!.GetServerList();

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(200, objectResult.StatusCode);

            var returnedServers = objectResult.Value as List<Server>;
            Assert.IsNotNull(returnedServers);
            Assert.AreEqual(2, returnedServers.Count);
            Assert.AreEqual("server1.com", returnedServers[0].Host);
            Assert.AreEqual("server2.com", returnedServers[1].Host);
        }

        [TestMethod]
        public async Task GetServerList_FileNotFound_ReturnsNotFound()
        {
            _mockServerService!.Setup(service => service.GetServersAsync())
                .ThrowsAsync(new FileNotFoundException("Servers file not found"));

            var result = await _serverController!.GetServerList();

            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("Servers file not found", notFoundResult.Value);
        }

        [TestMethod]
        public async Task GetServerList_ExceptionThrown_ReturnsInternalServerError()
        {
            _mockServerService!.Setup(service => service.GetServersAsync())
                .ThrowsAsync(new Exception("Test exception"));

            var result = await _serverController!.GetServerList();

            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("Server error: Test exception", statusCodeResult.Value);
        }

        [TestMethod]
        public async Task AddServer_ValidHost_ReturnsOkWithSuccessMessage()
        {
            const string host = "server3.com";
            var server = new Server { Host = host, Country = "USA", City = "Los Angeles" };

            _mockServerService!.Setup(service => service.AddServerAsync(host))
                .Returns(Task.CompletedTask);
            _mockServerService.Setup(service => service.GetServersAsync())
                .ReturnsAsync([server]);

            var result = await _serverController!.AddServer(host);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(200, objectResult.StatusCode);

            var response = objectResult.Value as string;
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Contains("Server added successfully"));
            Assert.IsTrue(response.Contains("server3.com"));
            Assert.IsTrue(response.Contains("Country: USA"));
            Assert.IsTrue(response.Contains("City: Los Angeles"));
        }

        [TestMethod]
        public async Task AddServer_EmptyHost_ReturnsBadRequest()
        {
            const string host = "";

            var result = await _serverController!.AddServer(host);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Enter correct data", badRequestResult.Value);
        }

        [TestMethod]
        public async Task AddServer_ExceptionThrown_ReturnsInternalServerError()
        {
            const string host = "server3.com";
            _mockServerService!.Setup(service => service.AddServerAsync(host))
                .ThrowsAsync(new Exception("Test exception"));

            var result = await _serverController!.AddServer(host);

            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("Server Error Test exception", statusCodeResult.Value);
        }

        [TestMethod]
        public async Task UpdateHost_ValidRequest_ReturnsOkWithSuccessMessage()
        {
            // Arrange
            const string oldHost = "server1.com";
            const string newHost = "server2.com";
            var request = new UpdateHostRequest { OldHost = oldHost, NewHost = newHost };
            var server = new Server { Host = oldHost, Country = "USA", City = "New York" };

            _mockServerService!.Setup(service => service.GetServersAsync())
                .ReturnsAsync([server]);
            _mockServerService.Setup(service => service.UpdateServerAsync(It.IsAny<List<Server>>()))
                .Returns(Task.CompletedTask);

            var result = await _serverController!.UpdateHost(request);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(200, objectResult.StatusCode);

            var response = objectResult.Value as string;
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Contains($"Old host {oldHost} update"));
            Assert.IsTrue(response.Contains($"to {newHost}"));
        }

        [TestMethod]
        public async Task UpdateHost_InvalidRequest_ReturnsBadRequest()
        {
            var request = new UpdateHostRequest { OldHost = "", NewHost = "" };

            var result = await _serverController!.UpdateHost(request);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Enter correct data", badRequestResult.Value);
        }

        [TestMethod]
        public async Task UpdateHost_ServerNotFound_ReturnsNotFound()
        {
            const string oldHost = "server1.com";
            const string newHost = "server2.com";
            var request = new UpdateHostRequest { OldHost = oldHost, NewHost = newHost };

            _mockServerService!.Setup(service => service.GetServersAsync())
                .ReturnsAsync([]);

            var result = await _serverController!.UpdateHost(request);

            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"Old host: {oldHost} not found", notFoundResult.Value);
        }

        [TestMethod]
        public async Task UpdateHost_ExceptionThrown_ReturnsInternalServerError()
        {
            const string oldHost = "server1.com";
            const string newHost = "server2.com";
            var request = new UpdateHostRequest { OldHost = oldHost, NewHost = newHost };

            _mockServerService!.Setup(service => service.GetServersAsync())
                .ReturnsAsync([new Server { Host = oldHost, Country = "USA", City = "New York" }]);
            _mockServerService.Setup(service => service.UpdateServerAsync(It.IsAny<List<Server>>()))
                .ThrowsAsync(new Exception("Test exception"));

            var result = await _serverController!.UpdateHost(request);

            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("Server error: Test exception", statusCodeResult.Value);
        }

        [TestMethod]
        public async Task DeleteServer_ValidRequest_ReturnsOk()
        {
            const string city = "New York";
            const string host = "server1.com";

            _mockServerService!.Setup(service => service.DeleteServerAsync(city, host))
                .Returns(Task.CompletedTask);

            var result = await _serverController!.DeleteServer(city, host);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(200, objectResult.StatusCode);
            var response = objectResult.Value as string;
            Assert.IsNotNull(response);
            Assert.AreEqual($"Server successfully deleted for city: {city} with host {host}", response);
        }

        [TestMethod]
        public async Task DeleteServer_MissingCity_ReturnsBadRequest()
        {
            const string city = "";
            const string host = "server1.com";

            var result = await _serverController!.DeleteServer(city, host);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("OldHost can't be null or empty", badRequestResult.Value);
        }

        [TestMethod]
        public async Task DeleteServer_ServerNotFound_ReturnsNotFound()
        {
            const string city = "New York";
            const string host = "server1.com";

            _mockServerService!.Setup(service => service.DeleteServerAsync(city, host))
                .ThrowsAsync(new InvalidOperationException($"Server with host {host} not found"));

            var result = await _serverController!.DeleteServer(city, host);

            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"Server with host {host} not found", notFoundResult.Value);
        }

        [TestMethod]
        public async Task DeleteServer_ArgumentException_ReturnsBadRequest()
        {
            const string city = "New York";
            const string host = "server1.com";
                
            _mockServerService!.Setup(service => service.DeleteServerAsync(city, host))
                .ThrowsAsync(new ArgumentException("Invalid argument"));

            var result = await _serverController!.DeleteServer(city, host);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Invalid argument", badRequestResult.Value);
        }

        [TestMethod]
        public async Task DeleteServer_ExceptionThrown_ReturnsInternalServerError()
        {
            const string city = "New York";
            const string host = "server1.com";

            _mockServerService!.Setup(service => service.DeleteServerAsync(city, host))
                .ThrowsAsync(new Exception("Test exception"));

            var result = await _serverController!.DeleteServer(city, host);

            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("Server error: Test exception", statusCodeResult.Value);
        }

        [TestMethod]
        public async Task DeleteAllServer_ValidRequest_ReturnsOk()
        {
            const string country = "USA";

            _mockServerService!.Setup(service => service.DeleteAllServerAsync(country))
                .Returns(Task.CompletedTask);

            var result = await _serverController!.DeleteAllServer(country);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(200, objectResult.StatusCode);
            var response = objectResult.Value as string;
            Assert.IsNotNull(response);
            Assert.AreEqual($"All servers for country: '{country}' successfully deleted", response);
        }

        [TestMethod]
        public async Task DeleteAllServer_MissingCountry_ReturnsBadRequest()
        {
            const string country = "";

            var result = await _serverController!.DeleteAllServer(country);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("OldHost can't be null or empty", badRequestResult.Value);
        }
        [TestMethod]

        public async Task DeleteAllServer_ServersNotFound_ReturnsNotFound()
        {
            const string country = "USA";

            _mockServerService!.Setup(service => service.DeleteAllServerAsync(country))
                .ThrowsAsync(new InvalidOperationException($"No servers found for country {country}"));

            var result = await _serverController!.DeleteAllServer(country);

            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"No servers found for country {country}", notFoundResult.Value);
        }

        [TestMethod]
        public async Task DeleteAllServer_ExceptionThrown_ReturnsInternalServerError()
        {
            const string country = "USA";

            _mockServerService!.Setup(service => service.DeleteAllServerAsync(country))
                .ThrowsAsync(new Exception("Test exception"));

            var result = await _serverController!.DeleteAllServer(country);

            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("Server error: Test exception", statusCodeResult.Value);
        }
    }
}
