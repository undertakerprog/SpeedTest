using Web.Src.Service;

namespace WebTest.Src.ServiceTest
{
    [TestClass]
    public class PingServiceTest
    {
        private PingService? _pingService;

        [TestInitialize]
        public void SetUp()
        {
            _pingService = new PingService();
        }

        [TestMethod]
        public async Task CheckPingAsync_ValidHost_ReturnsPingTime()
        {
            const string host = "www.google.com";
            const int timeout = 1000;

            var result = await _pingService!.CheckPingAsync(host, timeout);

            Assert.IsTrue(result is >= 0 and < double.MaxValue,
                "Ping should be non-negative and less than MaxValue for a valid host");
        }

        [TestMethod]
        public async Task CheckPingAsync_InvalidHost_ReturnsMaxValue()
        {
            const string host = "invalid-host-name";
            const int timeout = 1000;

            var result = await _pingService!.CheckPingAsync(host, timeout);

            Assert.AreEqual(double.MaxValue, result,
                "Ping time should be MaxValue for an invalid host");
        }

        [TestMethod]
        public async Task CheckPingAsync_Timeout_ReturnsMaxValue()
        {
            const string host = "10.255.255.1";
            const int timeout = 1;

            var result = await _pingService!.CheckPingAsync(host, timeout);

            Assert.AreEqual(double.MaxValue, result,
                "In case of timeout, the result should be MaxValue");
        }
    }
}
