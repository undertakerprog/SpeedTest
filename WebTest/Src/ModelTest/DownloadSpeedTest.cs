using Web.Src.Model;
using System.Text.Json;

namespace WebTest.Src.ModelTest
{
    [TestClass]
    public class DownloadSpeedTest
    {
        [TestMethod]
        public void DefaultValues_AreSetCorrectly()
        {
            var downloadSpeed = new DownloadSpeed();

            Assert.IsNotNull(downloadSpeed.Server, "Server must be initialize");
            Assert.AreEqual(0, downloadSpeed.Speed, "Speed must be 0 by default");
            Assert.AreEqual("unknown", downloadSpeed.Unit, "Unit must be 'unknown' by default");
            Assert.AreEqual(0, downloadSpeed.Ping, "Ping must be 0 by default");
            Assert.AreEqual("unknown", downloadSpeed.Source, "Source must be 'unknown' by default");
        }

        [TestMethod]
        public void Can_SetAndGetProperties()
        {
            var downloadSpeed = new DownloadSpeed();
            var server = new Server();

            downloadSpeed.Server = server;
            downloadSpeed.Speed = 100.5;
            downloadSpeed.Unit = "Mbps";
            downloadSpeed.Ping = 10.2;
            downloadSpeed.Source = "TestSource";

            Assert.AreEqual(server, downloadSpeed.Server, "Server must be installed");
            Assert.AreEqual(100.5, downloadSpeed.Speed, "Speed must be equal 100.5");
            Assert.AreEqual("Mbps", downloadSpeed.Unit, "Unit must be 'Mbps'");
            Assert.AreEqual(10.2, downloadSpeed.Ping, "Ping must be equal 10.2");
            Assert.AreEqual("TestSource", downloadSpeed.Source, "Source must be 'TestSource'");
        }

        [TestMethod]
        public void Can_SerializeAndDeserialize()
        {
            var downloadSpeed = new DownloadSpeed
            {
                Speed = 100.5,
                Unit = "Mbps",
                Ping = 10.2,
                Source = "TestSource"
            };

            var json = JsonSerializer.Serialize(downloadSpeed);
            var deserialized = JsonSerializer.Deserialize<DownloadSpeed>(json);

            Assert.IsNotNull(deserialized, "Deserialization should return the object");
            Assert.AreEqual(100.5, deserialized.Speed, "Speed must match after deserialization");
            Assert.AreEqual("Mbps", deserialized.Unit, "Unit must match after deserialization");
            Assert.AreEqual(10.2, deserialized.Ping, "Ping must match after deserialization");
            Assert.AreEqual("TestSource", deserialized.Source, "Source must match after deserialization");
        }
    }
}
