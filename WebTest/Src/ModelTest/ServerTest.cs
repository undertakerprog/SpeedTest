using Web.Src.Model;
using System.Text.Json;

namespace WebTest.Src.ModelTest
{
    [TestClass]
    public class ServerTest
    {
        [TestMethod]
        public void DefaultValues_AreSetCorrectly()
        {
            var server = new Server();

            Assert.AreEqual("unknown", server.Country, "Country must be 'unknown' by default");
            Assert.AreEqual("unknown", server.City, "Speed must be 'unknown' by default");
            Assert.AreEqual("unknown", server.Host, "Unit must be 'unknown' by default");
            Assert.AreEqual("unknown", server.Provider, "Ping must be 'unknown' by default");
            Assert.AreEqual(0, server.Latitude, "Ping must be 0 by default");
            Assert.AreEqual(0, server.Longitude, "Ping must be 0 by default");
        }

        [TestMethod]
        public void Can_SetAndGetProperties()
        {
            var server = new Server
            {
                Country = "TestCountry",
                City = "TestCity",
                Host = "test.host",
                Provider = "test provider",
                Latitude = 10.1010,
                Longitude = 10.1010
            };

            Assert.AreEqual("TestCountry", server.Country, "Country must be 'TestCountry' by default");
            Assert.AreEqual("TestCity", server.City, "Speed must be 'TestCity' by default");
            Assert.AreEqual("test.host", server.Host, "Unit must be 'test.host' by default");
            Assert.AreEqual("test provider", server.Provider, "Ping must be 'test provider' by default");
            Assert.AreEqual(10.1010, server.Latitude, "Ping must be 10.1010 by default");
            Assert.AreEqual(10.1010, server.Longitude, "Ping must be 10.1010 by default");
        }

        [TestMethod]
        public void Can_SerializeAndDeserialize()
        {
            var server = new Server
            {
                Country = "TestCountry",
                City = "TestCity",
                Host = "test.host",
                Provider = "test provider",
                Latitude = 10.1010,
                Longitude = 10.1010
            };

            var json = JsonSerializer.Serialize(server);
            var deserialized = JsonSerializer.Deserialize<DownloadSpeed>(json);

            Assert.IsNotNull(deserialized, "Deserialization should return the object");
            Assert.AreEqual("TestCountry", server.Country, "Country must be 'TestCountry' by default");
            Assert.AreEqual("TestCity", server.City, "Speed must be 'TestCity' by default");
            Assert.AreEqual("test.host", server.Host, "Unit must be 'test.host' by default");
            Assert.AreEqual("test provider", server.Provider, "Ping must be 'test provider' by default");
            Assert.AreEqual(10.1010, server.Latitude, "Ping must be 10.1010 by default");
            Assert.AreEqual(10.1010, server.Longitude, "Ping must be 10.1010 by default");
        }
    }
}
