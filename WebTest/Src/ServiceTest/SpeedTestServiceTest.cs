using Microsoft.Extensions.Configuration;
using Moq;
using System.Net;
using Web.Src.Model;
using Web.Src.Service;
using WebTest.Src.Helpers;

namespace WebTest.Src.ServiceTest
{
    [TestClass]
    public class SpeedTestServiceTest
    {
        private Mock<IConfiguration>? _mockConfiguration;
        private Mock<ILocationService>? _mockLocationService;
        private HttpClient? _mockHttpClient;
        private SpeedTestService? _speedTestService;

        [TestInitialize]
        public void Setup()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLocationService = new Mock<ILocationService>();

            _mockConfiguration.Setup(c => c["SpeedTest:DownloadUrl"])
                .Returns("http://example.com/testfile");

            _mockHttpClient = CreateMockHttpClient(new string('0', 1024 * 1024));
            _speedTestService = new SpeedTestService(_mockConfiguration.Object,
                _mockLocationService.Object,
                _mockHttpClient);
        }

        [TestMethod]
        public async Task FastDownloadSpeedAsync_ValidConfiguration_ReturnsPositiveSpeed()
        {
            var duration = TimeSpan.FromSeconds(5);

            var result = await _speedTestService.FastDownloadSpeedAsync(duration);

            Assert.IsTrue(result > 0, "Download speed should be greater than zero.");
        }

        [TestMethod]
        public async Task FastDownloadSpeedAsync_MissingUrl_ThrowsInvalidOperationException()
        {
            _mockConfiguration!.Setup(c => c["SpeedTest:DownloadUrl"]).Returns((string)null!);

            var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                async () => await _speedTestService!.FastDownloadSpeedAsync(TimeSpan.FromSeconds(5))
            );
            Assert.AreEqual("Download URL isn't configured", exception.Message);
        }

        [TestMethod]
        public async Task GetDownloadSpeed_ValidConfiguration_ReturnsDownloadSpeed()
        {
            var server = new Server { Host = "example.com" };
            _mockLocationService!.Setup(l => l.GetBestServerAsync())
                .ReturnsAsync(server);

            var downloadUrls = new List<string> { "http://example.com/random1x1.jpg", "http://example.com/random2x2.jpg" };
            _speedTestService = new SpeedTestService(_mockConfiguration!.Object,
                _mockLocationService.Object,
                _mockHttpClient!);

            var result = await _speedTestService.GetDownloadSpeed();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Speed > 0, "Download speed should be greater than zero.");
        }

        private static HttpClient CreateMockHttpClient(string responseContent,
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var messageHandler = new FakeHttpMessageHandler(responseContent, statusCode);
            return new HttpClient(messageHandler)
            {
                BaseAddress = new Uri("http://example.com/")
            };
        }
    }
}