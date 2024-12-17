using Infrastructure;
using Moq;
using Web.Src.Service;

namespace WebTest.Src.ServiceTest
{
    [TestClass]
    public class ServerServiceTest
    {
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
    }
}
