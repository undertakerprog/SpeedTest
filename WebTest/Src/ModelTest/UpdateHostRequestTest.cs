using Web.Src.Model;
using System.Text.Json;

namespace WebTest.Src.ModelTest
{
    [TestClass]
    public class UpdateHostRequestTest
    {
        [TestMethod]
        public void DefaultValues_AreSetCorrectly()
        {
            var updateHost = new UpdateHostRequest();

            Assert.AreEqual("unknown", updateHost.OldHost, 
                "Old host must be 'unknown' by default");
            Assert.AreEqual("unknown", updateHost.NewHost, 
                "New host must be 'unknown' by default");
        }

        [TestMethod]
        public void Can_SetAndGetProperties()
        {
            var updateHost = new UpdateHostRequest
            {
                OldHost = "old.host.test",
                NewHost = "new.host.test",
            };

            Assert.AreEqual("old.host.test", updateHost.OldHost, 
                "Old host must be 'old.host.test' by default");
            Assert.AreEqual("new.host.test", updateHost.NewHost, 
                "New host must be 'new.host.test' by default");

        }

        [TestMethod]
        public void Can_SerializeAndDeserialize()
        {
            var updateHost = new UpdateHostRequest
            {
                OldHost = "old.host.test",
                NewHost = "new.host.test",
            };

            var json = JsonSerializer.Serialize(updateHost);
            var deserialized = JsonSerializer.Deserialize<DownloadSpeed>(json);

            Assert.IsNotNull(deserialized, "Deserialization should return the object");
            Assert.AreEqual("old.host.test", updateHost.OldHost, 
                "Old host must be 'old.host.test' by default");
            Assert.AreEqual("new.host.test", updateHost.NewHost, 
                "New host must be 'new.host.test' by default");
        }
    }
}
