using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Src.Model;
using Web.Src.Model.SpeedTest;
using Web.Src.Service;
using Web.Src.Сontroller;

namespace WebTest.Src.ControllerTest
{
    [TestClass]
    public class SpeedTestControllerTest
    {
        private Mock<ISpeedTestService>? _mockSpeedTestService;
        private SpeedTestController? _speedTestController;

        [TestInitialize]
        public void Setup()
        {
            _mockSpeedTestService = new Mock<ISpeedTestService>();
            _speedTestController = new SpeedTestController(_mockSpeedTestService.Object);
        }

        [TestMethod]
        public async Task GetDownloadSpeed_ValidHost_ReturnsOk()
        {
            const string host = "example.com";
            const double expectedSpeed = 50.5;
            var downloadSpeed = new DownloadSpeed
            {
                Speed = expectedSpeed,
                Unit = "Mbps",
                Ping = 10.0,
                Source = "test"
            };

            _mockSpeedTestService!.Setup(service => service.GetDownloadSpeed(host))
                .ReturnsAsync(downloadSpeed);

            var result = await _speedTestController!.GetDownloadSpeed(host);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var actualResult = okResult.Value as DownloadSpeed;
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(expectedSpeed, actualResult.Speed);
            Assert.AreEqual("Mbps", actualResult.Unit);
        }

        [TestMethod]
        public async Task GetDownloadSpeed_HostNotProvided_ReturnsInternalServerError()
        {
            string? host = null;
            _mockSpeedTestService!.Setup(service => service.GetDownloadSpeed(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Test exception"));

            var result = await _speedTestController!.GetDownloadSpeed(host);

            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("Internal server error: Test exception", statusCodeResult.Value);
        }

        [TestMethod]
        public async Task GetFastDownloadSpeed_Success_ReturnsOk()
        {
            const double expectedSpeed = 100.0;
            _mockSpeedTestService!.Setup(service => service.FastDownloadSpeedAsync(It.IsAny<TimeSpan>()))
                .ReturnsAsync(expectedSpeed);

            var result = await _speedTestController!.GetFastDownloadSpeed();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as DownloadSpeedResponse;
            Assert.IsNotNull(response);
            Assert.AreEqual(expectedSpeed, response.Speed);
        }

        [TestMethod]
        public async Task GetFastDownloadSpeed_ErrorThrown_ReturnsInternalServerError()
        {
            _mockSpeedTestService!.Setup(service => service.FastDownloadSpeedAsync(It.IsAny<TimeSpan>()))
                .ThrowsAsync(new Exception("Test exception"));

            var result = await _speedTestController!.GetFastDownloadSpeed();

            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("Server error: Test exception", statusCodeResult.Value);
        }
    }
}
