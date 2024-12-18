using Infrastructure;
using Moq;
using Newtonsoft.Json;
using Web.Src.Model;
using Web.Src.Service;

namespace WebTest.Src.ServiceTest
{
    [TestClass]
    public class ServerServiceTest
    {
        private const string FilePath = "..//Web//server.json";

        [TestMethod]
        public async Task GetServersAsync_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
        {
            var mockFileReader = new Mock<IFileReader>();
            mockFileReader.Setup(f => f.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

            var serverService = new ServerService(mockFileReader.Object);

            await Assert.ThrowsExceptionAsync<FileNotFoundException>(() => serverService.GetServersAsync());
        }

        [TestMethod]
        public async Task GetServersAsync_ShouldReturnListOfServers_WhenFileExists()
        {
            var mockFileReader = new Mock<IFileReader>();
            const string mockJsonData = "[{\"Latitude\": 10.0, \"Longitude\": 10.0, \"City\": \"TestCity1\", \"Host\": \"Server1\"}, " +
                                        "{\"Latitude\": 20.0, \"Longitude\": 20.0, \"City\": \"TestCity2\", \"Host\": \"Server2\"}]";

            mockFileReader.Setup(f => f.ExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            mockFileReader.Setup(f => f.ReadAllTextAsync(It.IsAny<string>())).ReturnsAsync(mockJsonData);

            var serverService = new ServerService(mockFileReader.Object);

            var result = await serverService.GetServersAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Server1", result[0].Host);
            Assert.AreEqual("Server2", result[1].Host);
        }

        [TestMethod]
        public async Task GetServersAsync_ShouldReturnEmptyList_WhenFileIsEmptyOrInvalid()
        {
            var mockFileReader = new Mock<IFileReader>();
            mockFileReader.Setup(f => f.ExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            mockFileReader.Setup(f => f.ReadAllTextAsync(It.IsAny<string>())).ReturnsAsync("[]");

            var serverService = new ServerService(mockFileReader.Object);

            var result = await serverService.GetServersAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetServersAsync_ShouldReturnEmptyList_WhenJsonIsInvalid()
        {
            var mockFileReader = new Mock<IFileReader>();
            mockFileReader.Setup(f => f.ExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            mockFileReader.Setup(f => f.ReadAllTextAsync(It.IsAny<string>())).ReturnsAsync("{ \"someProperty\": \"someValue\" }");

            var serverService = new ServerService(mockFileReader.Object);

            var result = await serverService.GetServersAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetServersAsync_ShouldReturnEmptyList_WhenJsonDeserializationReturnsNull()
        {
            var mockFileReader = new Mock<IFileReader>();
            const string mockJsonData = "null";

            mockFileReader.Setup(f => f.ExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            mockFileReader.Setup(f => f.ReadAllTextAsync(It.IsAny<string>())).ReturnsAsync(mockJsonData);

            var serverService = new ServerService(mockFileReader.Object);

            var result = await serverService.GetServersAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task UpdateServerAsync_ShouldWriteJsonToFile_WhenServersListIsProvided()
        {
            var mockFileReader = new Mock<IFileReader>();

            var servers = new List<Server>
            {
                new () { Host = "server1", Latitude = 10.0, Longitude = 20.0, Country = "Country1", City = "City1" },
                new () { Host = "server2", Latitude = 30.0, Longitude = 40.0, Country = "Country2", City = "City2" }
            };

            mockFileReader.Setup(f => f.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var serverService = new ServerService(mockFileReader.Object);

            await serverService.UpdateServerAsync(servers);

            mockFileReader.Verify(f => f.WriteAllTextAsync(FilePath, It.Is<string>(s => s.Contains("server1") && s.Contains("server2"))), Times.Once);
        }

        [TestMethod]
        public async Task DeleteServerAsync_RemovesServer_WhenCityHasOneServer()
        {
            var mockFileReader = new Mock<IFileReader>();

            var mockServers = new List<Server>
            {
                new () { Host = "host1", City = "City1" },
                new () { Host = "host2", City = "City1" },
                new () { Host = "host3", City = "City2" }
            };

            mockFileReader.Setup(service => service.ExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            mockFileReader.Setup(service => service.ReadAllTextAsync(It.IsAny<string>()))
                .ReturnsAsync(JsonConvert.SerializeObject(mockServers));

            var serverService = new ServerService(mockFileReader.Object);

            await serverService.DeleteServerAsync("City2");

            var updatedServers = mockServers.Where(s => s.City != "City2").ToList();

            Assert.AreEqual(2, updatedServers.Count);
            Assert.IsFalse(updatedServers.Any(s => s.City == "City2"));

            mockFileReader.Verify(f => f.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task DeleteServerAsync_RemovesSpecificServer_WhenCityHasMultipleServers()
        {
            var mockFileReader = new Mock<IFileReader>();

            var mockServers = new List<Server>
            {
                new () { Host = "host1", City = "City1" },
                new () { Host = "host2", City = "City1" },
                new () { Host = "host3", City = "City2" }
            };

            mockFileReader.Setup(service => service.ExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            mockFileReader.Setup(service => service.ReadAllTextAsync(It.IsAny<string>()))
                .ReturnsAsync(JsonConvert.SerializeObject(mockServers));

            var serverService = new ServerService(mockFileReader.Object);

            await serverService.DeleteServerAsync("City1", "host1");

            var updatedServers = mockServers.Where(s => s is not { City: "City1", Host: "host1" }).ToList();

            Assert.AreEqual(2, updatedServers.Count);
            Assert.IsFalse(updatedServers.Any(s => s.Host == "host1"));

            mockFileReader.Verify(f => f.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task DeleteAllServerAsync_RemovesAllServersForCountry()
        {
            var mockFileReader = new Mock<IFileReader>();
            var mockServers = new List<Server>
            {
                new() { Host = "host1", City = "City1", Country = "Country1" },
                new() { Host = "host2", City = "City2", Country = "Country1" },
                new() { Host = "host3", City = "City3", Country = "Country2" }
            };

            mockFileReader.Setup(f => f.ExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

            mockFileReader.Setup(f => f.ReadAllTextAsync(It.IsAny<string>())).
                ReturnsAsync(JsonConvert.SerializeObject(mockServers));

            var serverService = new ServerService(mockFileReader.Object);

            await serverService.DeleteAllServerAsync("Country1");

            var expectedServers = mockServers
                .Where(s => !s.Country.Equals("Country1", StringComparison.OrdinalIgnoreCase))
                .ToList();

            var jsonData = JsonConvert.SerializeObject(expectedServers, Formatting.Indented);

            mockFileReader.Verify(f => f.WriteAllTextAsync(It.IsAny<string>(), jsonData), Times.Once,
                "Метод записи файла должен быть вызван с обновленными данными.");

            Assert.AreEqual(1, expectedServers.Count);
            Assert.IsFalse(expectedServers.Any(s => s.Country == "Country1"));
        }

        [TestMethod]
        public async Task DeleteAllServerAsync_ThrowsException_WhenNoServersForCountry()
        {
            var mockFileReader = new Mock<IFileReader>();
            var mockServers = new List<Server>
            {
                new () { Host = "host1", City = "City1", Country = "Country2" },
                new () { Host = "host2", City = "City2", Country = "Country2" }
            };

            mockFileReader.Setup(f => f.ExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            mockFileReader.Setup(f => f.ReadAllTextAsync(It.IsAny<string>()))
                .ReturnsAsync(JsonConvert.SerializeObject(mockServers));

            var serverService = new ServerService(mockFileReader.Object);
            var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                async () => await serverService.DeleteAllServerAsync("Country1"));

            Assert.AreEqual("Server for country: Country1 not found", exception.Message);

            mockFileReader.Verify(f => f.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
