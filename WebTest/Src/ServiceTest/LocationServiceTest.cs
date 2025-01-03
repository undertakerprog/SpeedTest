﻿using System.Net;
using Moq;
using Web.Src.Model;
using Web.Src.Service;
using WebTest.Src.Helpers;

namespace WebTest.Src.ServiceTest
{
    [TestClass]
    public class LocationServiceTest
    {
        private LocationService? _locationService;
        private const string Host = "8.8.8.8";
        private const string InvalidHost = "invalid-ip";

        [TestMethod]
        public async Task GetUserLocationAsync_ValidResponse_ReturnsCorrectLocation()
        {
            const string jsonResponse = """
                                        {
                                            "lat": 40.7128,
                                            "lon": -74.0060,
                                            "country": "United States",
                                            "city": "New York",
                                            "query": "192.168.0.1"
                                        }
                                        """;

            var httpClient = CreateMockHttpClient(jsonResponse);
            _locationService = new LocationService(httpClient);

            var result = await _locationService.GetUserLocationAsync();

            Assert.AreEqual(40.7128, result.Latitude, 
                "Latitude should match the mocked value");
            Assert.AreEqual(-74.0060, result.Longitude, 
                "Longitude should match the mocked value");
            Assert.AreEqual("United States", result.Country, 
                "Country should match the mocked value");
            Assert.AreEqual("New York", result.City, 
                "City should match the mocked value");
            Assert.AreEqual("192.168.0.1", result.Query, 
                "Query should match the mocked value");
        }

        [TestMethod]
        public async Task GetUserLocationAsync_MissingFields_ReturnsDefaultValues()
        {
            const string jsonResponse = """
                                        {
                                            "lat": 0,
                                            "lon": 0
                                        }
                                        """;

            var httpClient = CreateMockHttpClient(jsonResponse);
            _locationService = new LocationService(httpClient);

            var result = await _locationService.GetUserLocationAsync();

            Assert.AreEqual(0, result.Latitude, 
                "Latitude should default to 0");
            Assert.AreEqual(0, result.Longitude, 
                "Latitude should default to 0");
            Assert.AreEqual("Unknown country", result.Country, 
                "Country should default to 'Unknown country'");
            Assert.AreEqual("Unknown city", result.City, 
                "City should default to 'Unknown city'");
            Assert.AreEqual("Unknown query", result.Query, 
                "Query should default to 'Unknown query'");
        }

        [TestMethod]
        public async Task GetUserLocationAsync_ApiError_ReturnsDefaultValues()
        {
            var httpClient = CreateMockHttpClient("Internal Server Error", HttpStatusCode.InternalServerError);
            _locationService = new LocationService(httpClient);

            var result = await _locationService.GetUserLocationAsync();

            Assert.AreEqual(0, result.Latitude, 
                "Latitude should default to 0 on API error");
            Assert.AreEqual(0, result.Longitude, 
                "Longitude should default to 0 on API error");
            Assert.AreEqual("Unknown country", result.Country, 
                "Country should default to 'Unknown country' on API error");
            Assert.AreEqual("Unknown city", result.City, 
                "City should default to 'Unknown city' on API error");
            Assert.AreEqual("Unknown query", result.Query, 
                "City should default to 'Unknown query' on API error");
        }

        [TestMethod]
        public async Task GetLocationByIpAsync_ValidResponse_ReturnsCorrectLocation()
        {
            const string jsonResponse = """
                                        {
                                            "lat": 40.7128,
                                            "lon": -74.0060,
                                            "country": "United States",
                                            "city": "New York"
                                        }
                                        """;

            var httpClient = CreateMockHttpClient(jsonResponse);
            _locationService = new LocationService(httpClient);

            var result = await _locationService.GetLocationByIpAsync(Host);

            Assert.AreEqual(40.7128, result.Latitude, 
                "Latitude should match the mocked value");
            Assert.AreEqual(-74.0060, result.Longtitude, 
                "Longitude should match the mocked value");
            Assert.AreEqual("United States", result.Country, 
                "Country should match the mocked value");
            Assert.AreEqual("New York", result.City, 
                "City should match the mocked value");
        }

        [TestMethod]
        public async Task GetLocationByIpAsync_MissingFields_ReturnsDefaultValues()
        {
            const string jsonResponse = """
                                        {
                                            "lat": 0,
                                            "lon": 0
                                        }
                                        """;

            var httpClient = CreateMockHttpClient(jsonResponse);
            _locationService = new LocationService(httpClient);

            var result = await _locationService.GetLocationByIpAsync(Host);

            Assert.AreEqual(0, result.Latitude, 
                "Latitude should default to 0");
            Assert.AreEqual(0, result.Longtitude, 
                "Longitude should default to 0");
            Assert.AreEqual("Unknown country", result.Country, 
                "Country should default to 'Unknown county'");
            Assert.AreEqual("Unknown city", result.City, 
                "City should default to 'Unknown county'");
        }

        [TestMethod]
        public async Task GetLocationByIpAsync_InvalidIp_ReturnsDefaultValues()
        {
            const string jsonResponse = """
                                        {
                                            "status": "fail",
                                            "message": "invalid query"
                                        }
                                        """;

            var httpClient = CreateMockHttpClient(jsonResponse, HttpStatusCode.BadRequest);
            _locationService = new LocationService(httpClient);

            var result = await _locationService.GetLocationByIpAsync(InvalidHost);

            Assert.AreEqual(0, result.Latitude, 
                "Latitude should default to 0 for invalid IP");
            Assert.AreEqual(0, result.Longtitude, 
                "Longitude should default to 0 for invalid IP");
            Assert.AreEqual("Unknown country", result.Country, 
                "Country should default to 'Unknown county' for invalid IP");
            Assert.AreEqual("Unknown city", result.City, 
                "City should default to 'Unknown county' for invalid IP");
        }

        [TestMethod]
        public async Task GetLocationByIpAsync_ApiError_ReturnsDefaultValues()
        {
            var httpClient = CreateMockHttpClient("Internal Server Error", HttpStatusCode.InternalServerError);
            _locationService = new LocationService(httpClient);

            var result = await _locationService.GetLocationByIpAsync(Host);

            Assert.AreEqual(0, result.Latitude, 
                "Latitude should default to 0 on API error");
            Assert.AreEqual(0, result.Longtitude, 
                "Longitude should default to 0 on API error");
            Assert.AreEqual("Unknown country"
                , result.Country, "Country should default to 'Unknown county' on API error");
            Assert.AreEqual("Unknown city", result.City, 
                "City should default to 'Unknown county' on API error");
        }

        [TestMethod]
        public async Task GetServersByCityAsync_CityIsNullOrWhitespace_ReturnsAllServers()
        {
            var servers = new List<Server>
            {
                new() { Country = "USA", City = "New York", Host = "host1", 
                    Provider = "provider1", Latitude = 40.7128, Longitude = -74.0060 },
                new() { Country = "Canada", City = "Toronto", Host = "host2",
                    Provider = "provider2", Latitude = 43.65107, Longitude = -79.347015 },
                new() { Country = "Germany", City = "Berlin", Host = "host3", 
                    Provider = "provider3", Latitude = 52.5200, Longitude = 13.4050 }
            };
            var mockLocationService = CreateMockLocationService(servers);

            var result = await mockLocationService.Object.GetServersByCityAsync(string.Empty);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count, "The number of servers should match the test data");
        }

        [TestMethod]
        public async Task GetServersByCityAsync_CityIsSpecified_ReturnsFilteredServers()
        {
            var servers = new List<Server>
            {
                new() { Country = "USA", City = "New York", Host = "host1", Provider = "provider1", Latitude = 40.7128, Longitude = -74.0060 },
                new() { Country = "Canada", City = "Toronto", Host = "host2", Provider = "provider2", Latitude = 43.65107, Longitude = -79.347015 },
                new() { Country = "Germany", City = "Berlin", Host = "host3", Provider = "provider3", Latitude = 52.5200, Longitude = 13.4050 }
            };
            var mockLocationService = CreateMockLocationService(servers);

            var result = await mockLocationService.Object.GetServersByCityAsync("New York");

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count, "Only one server should be returned for New York");
            Assert.AreEqual("New York",  result[0].City, 
                "The returned server should be from New York");
        }

        [TestMethod]
        public async Task GetServersByCityAsync_NoServersInFile_ReturnsEmptyList()
        {
            var mockLocationService = CreateMockLocationService([]);

            var result = await mockLocationService.Object.GetServersByCityAsync(string.Empty);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count,
                "The number of servers should be zero when there are no servers in the file");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task GetServersByCityAsync_FileReadErrorThrowsException()
        {
            var mockLocationService = new Mock<LocationService>(Mock.Of<HttpClient>());
            mockLocationService
                .Setup(x => x.LoadServersAsync())
                .ThrowsAsync(new Exception("No server available in the list"));

            await mockLocationService.Object.GetServersByCityAsync(string.Empty);
        }

        [TestMethod]
        public async Task GetClosestServerAsync_ShouldReturnClosestServerInSameCity()
        {
            var testServers = new List<Server>
            {
                new () { Latitude = 10.0, Longitude = 10.0, City = "TestCity1", Host = "Server1" },
                new () { Latitude = 15.0, Longitude = 15.0, City = "TestCity2", Host = "Server2" }
            };

            var mockService = CreateMockLocationService(testServers);

            mockService.Setup(x => x.GetUserLocationAsync()).
                ReturnsAsync((10.0, 10.0, "TestCountry", "TestCity", "127.0.0.1"));

            var result = await mockService.Object.GetClosestServerAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual("Server1", result.Host);
        }

        [TestMethod]
        public async Task GetClosestServerAsync_ShouldReturnClosestServerIfNoSameCity()
        {
            var testServers = new List<Server>
            {
                new () { Latitude = 20.0, Longitude = 20.0, City = "OtherCity1", Host = "Server3" },
                new () { Latitude = 25.0, Longitude = 25.0, City = "OtherCity2", Host = "Server4" }
            };

            var mockService = CreateMockLocationService(testServers);

            mockService.Setup(x => x.GetUserLocationAsync()).
                ReturnsAsync((10.0, 10.0, "TestCountry", "TestCity", "127.0.0.1"));

            var result = await mockService.Object.GetClosestServerAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual("Server3", result.Host);
        }

        [TestMethod]
        public async Task GetClosestServerAsync_ShouldReturnNullIfNoServersAvailable()
        {
            var testServers = new List<Server>();

            var mockService = CreateMockLocationService(testServers);

            mockService.Setup(x => x.GetUserLocationAsync()).
                ReturnsAsync((10.0, 10.0, "TestCountry", "TestCity", "127.0.0.1"));

            var result = await mockService.Object.GetClosestServerAsync();

            Assert.IsNull(result);
        }

        private static Mock<LocationService> CreateMockLocationService(List<Server> servers)
        {
            var mockLocationService = new Mock<LocationService>(Mock.Of<HttpClient>());
            mockLocationService.Setup(x => x.LoadServersAsync()) 
                .ReturnsAsync(servers);
            return mockLocationService;
        }

        [TestMethod]
        public async Task LoadServersAsync_ShouldReturnListOfServers_WhenUsingRealFile()
        {
            var expectedServers = new List<Server>
            {
                new () { Latitude = 10.0, Longitude = 10.0, City = "TestCity1", Host = "Server1" },
                new () { Latitude = 20.0, Longitude = 20.0, City = "TestCity2", Host = "Server2" }
            };

            var mockLocationService = CreateMockLocationService(expectedServers);

            var result = await mockLocationService.Object.LoadServersAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedServers.Count, result.Count);
            Assert.AreEqual(expectedServers[0].Host, result[0].Host);
            Assert.AreEqual(expectedServers[1].Host, result[1].Host);
        }

        private static HttpClient CreateMockHttpClient(string responseContent,
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var messageHandler = new FakeHttpMessageHandler(responseContent, statusCode);
            return new HttpClient(messageHandler);
        }
    }
}
