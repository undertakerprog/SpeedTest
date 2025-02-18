using Web.Src.Model.SpeedTest;

namespace WebTest.Src.ModelTest.SpeedTest
{
    [TestClass]
    public class DownloadSpeedResponseTests
    {
        [TestMethod]
        public void DownloadSpeedResponse_Constructor_ShouldInitializeSpeedMbps()
        {
            const double speedMbps = 100.5;

            var downloadSpeedResponse = new DownloadSpeedResponse
            {
                Speed = speedMbps
            };

            Assert.AreEqual(speedMbps, downloadSpeedResponse.Speed);
        }

        [TestMethod]
        public void DownloadSpeedResponse_Constructor_ShouldHandleZeroSpeed()
        {
            const double speedMbps = 0.0;

            var downloadSpeedResponse = new DownloadSpeedResponse
            {
                Speed = speedMbps
            };

            Assert.AreEqual(speedMbps, downloadSpeedResponse.Speed);
        }

        [TestMethod]
        public void DownloadSpeedResponse_Constructor_ShouldHandleNegativeSpeed()
        {
            const double speedMbps = -50.0;

            var downloadSpeedResponse = new DownloadSpeedResponse
            {
                Speed = speedMbps
            };

            Assert.AreEqual(speedMbps, downloadSpeedResponse.Speed);
        }

        [TestMethod]
        public void DownloadSpeedResponse_Constructor_ShouldHandleLargeSpeed()
        {
            const double speedMbps = 10000.0;

            var downloadSpeedResponse = new DownloadSpeedResponse
            {
                Speed = speedMbps
            };

            Assert.AreEqual(speedMbps, downloadSpeedResponse.Speed);
        }
    }
}
