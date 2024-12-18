using Web.Src.Model.Ping;

namespace WebTest.Src.ModelTest.Ping
{
    [TestClass]
    public class PingResponseTests
    {
        [TestMethod]
        public void PingResponse_Constructor_ShouldInitializeProperties()
        {
            const string host = "example.com";
            const double pingResult = 123.45;

            var pingResponse = new PingResponse
            {
                Host = host,
                PingResult = pingResult
            };

            Assert.AreEqual(host, pingResponse.Host);
            Assert.AreEqual(pingResult, pingResponse.PingResult);
        }

        [TestMethod]
        public void PingResponse_Constructor_ShouldHandleNullHost()
        {
            string? host = null;
            const double pingResult = 123.45;

            var pingResponse = new PingResponse
            {
                Host = host,
                PingResult = pingResult
            };

            Assert.IsNull(pingResponse.Host);
            Assert.AreEqual(pingResult, pingResponse.PingResult);
        }

        [TestMethod]
        public void PingResponse_Constructor_ShouldHandleZeroPingResult()
        {
            const string host = "example.com";
            const double pingResult = 0.0;

            var pingResponse = new PingResponse
            {
                Host = host,
                PingResult = pingResult
            };

            Assert.AreEqual(host, pingResponse.Host);
            Assert.AreEqual(pingResult, pingResponse.PingResult);
        }

        [TestMethod]
        public void PingResponse_Constructor_ShouldHandleNegativePingResult()
        {
            const string host = "example.com";
            const double pingResult = -123.45;

            var pingResponse = new PingResponse
            {
                Host = host,
                PingResult = pingResult
            };

            Assert.AreEqual(host, pingResponse.Host);
            Assert.AreEqual(pingResult, pingResponse.PingResult);
        }
    }
}
