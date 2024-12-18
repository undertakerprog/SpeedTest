using Web.Src.Model.Location;
using Web.Src.Model;

namespace WebTest.Src.ModelTest.LocationTest
{
    [TestClass]
    public class ServerResponseTest
    {
        [TestMethod]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            const string? message = "Success";
            var server = new Server
            {
                Country = "Country1",
                City = "City1",
                Host = "192.168.1.1",
                Provider = "Provider1",
                Latitude = 10.1,
                Longitude = 20.2
            };

            var serverResponse = new ServerResponse
            {
                Message = message,
                Server = server
            };

            Assert.AreEqual(message, serverResponse.Message);
            Assert.AreEqual(server, serverResponse.Server);
        }

        [TestMethod]
        public void Message_ShouldBeNullable()
        {
            string? message = null;

            var serverResponse = new ServerResponse
            {
                Message = message
            };

            Assert.IsNull(serverResponse.Message);
        }

        [TestMethod]
        public void Server_ShouldBeNullable()
        {
            Server? server = null;

            var serverResponse = new ServerResponse
            {
                Server = server
            };

            Assert.IsNull(serverResponse.Server);
        }
    }
}
