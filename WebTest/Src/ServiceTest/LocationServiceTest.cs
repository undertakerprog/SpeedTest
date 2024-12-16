using Web.Src.Service;
using WebTest.Src.Helpers;

namespace WebTest.Src.ServiceTest
{
    [TestClass]
    public class LocationServiceTest
    {
        private LocationService? _locationService;

        private static HttpClient CreateMockHttpClient(string responseContent)
        {
            var messageHandler = new FakeHttpMessageHandler(responseContent);
            return new HttpClient(messageHandler);
        }

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
    }
}
