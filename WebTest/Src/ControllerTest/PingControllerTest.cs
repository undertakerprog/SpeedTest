using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Src.Model.Ping;
using Web.Src.Service;
using Web.Src.Сontroller;

namespace WebTest.Src.ControllerTest
{
    [TestClass]
    public class PingControllerTest
    {
        private Mock<IPingService>? _mockPingService;
        private PingController? _pingController;

        [TestInitialize]
        public void Setup()
        {
            _mockPingService = new Mock<IPingService>();
            _pingController = new PingController(_mockPingService.Object);
        }

        [TestMethod]
        public async Task PostPingHost_ValidHost_ReturnsOkWithPingResult()
        {
            const string host = "example.com";
            const double pingResult = 50.5;
            const int timeout = 5000;

            _mockPingService!.Setup(s => s.CheckPingAsync(host, timeout)).ReturnsAsync(pingResult);

            var result = await _pingController!.PostPingHost(host);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as PingResponse;
            Assert.IsNotNull(response);
            Assert.AreEqual(host, response.Host);
            Assert.AreEqual(pingResult, response.PingResult);
        }

        [TestMethod]
        public async Task PostPingHost_HostIsNull_ReturnsBadRequest()
        {
            string? host = null;

            var result = await _pingController!.PostPingHost(host);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Host can't be null or empty", badRequestResult.Value);
        }

        [TestMethod]
        public async Task PostPingHost_HostIsEmpty_ReturnsBadRequest()
        {
            const string host = "";

            var result = await _pingController!.PostPingHost(host);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Host can't be null or empty", badRequestResult.Value);
        }

        [TestMethod]
        public async Task PostPingHost_ExceptionThrown_ReturnsInternalServerError()
        {
            const string host = "example.com";
            const int timeout = 5000;

            _mockPingService!.Setup(s => s.CheckPingAsync(host, timeout)).ThrowsAsync(new Exception("Test exception"));

            var result = await _pingController!.PostPingHost(host);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsTrue(objectResult.Value!.ToString()!.Contains("Test exception"));
        }
    }
}
