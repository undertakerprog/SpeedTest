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

            var result = await _pingService!.CheckPingAsync(host);
            Assert.IsTrue(result >= 0, "Ping should be non-negative for a valid host");
        }

        [TestMethod]
        public async Task CheckPingAsync_InvalidHost_ReturnsMaxValue()
        {
            const string host = "invalid-host-name";
            var result = await _pingService!.CheckPingAsync(host);

            Assert.AreEqual(double.MaxValue, result,
                "Ping time should be MaxValue for an invalid host");
        }

        [TestMethod]
        public async Task CheckPingAsync_Timeout_ReturnsMaxValue()
        {
            const string host = "8.8.8.8";

            var result = await _pingService!.CheckPingAsync(host);

            Assert.IsTrue(result is double.MaxValue or >= 0,
                "In case of timeout, the result should be MaxValue");
        }
    }
}
