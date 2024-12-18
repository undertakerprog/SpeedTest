using Web.Src.Model.Ping;

namespace WebTest.Src.ModelTest.Ping
{
    [TestClass]
    public class PingErrorResponseTests
    {
        [TestMethod]
        public void PingErrorResponse_Constructor_ShouldInitializeProperties()
        {
            const string host = "example.com";
            const string message = "Ping failed due to timeout.";

            var pingErrorResponse = new PingErrorResponse
            {
                Host = host,
                Message = message
            };

            Assert.AreEqual(host, pingErrorResponse.Host);
            Assert.AreEqual(message, pingErrorResponse.Message);
        }

        [TestMethod]
        public void PingErrorResponse_Constructor_ShouldHandleNullValues()
        {
            string? host = null;
            string? message = null;

            var pingErrorResponse = new PingErrorResponse
            {
                Host = host,
                Message = message
            };

            Assert.IsNull(pingErrorResponse.Host);
            Assert.IsNull(pingErrorResponse.Message);
        }

        [TestMethod]
        public void PingErrorResponse_Constructor_ShouldHandlePartialInitialization()
        {
            const string host = "example.com";
            string? message = null;

            var pingErrorResponse = new PingErrorResponse
            {
                Host = host,
                Message = message
            };

            Assert.AreEqual(host, pingErrorResponse.Host);
            Assert.IsNull(pingErrorResponse.Message);
        }
    }
}
