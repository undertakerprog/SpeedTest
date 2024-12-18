using Web.Src.Model.Location;

namespace WebTest.Src.ModelTest.LocationTest
{
    [TestClass]
    public class UserLocationResponseTests
    {
        [TestMethod]
        public void UserLocationResponse_Constructor_ShouldInitializeProperties()
        {
            const double latitude = 52.5200;
            const double longitude = 13.4050;
            const string country = "Germany";
            const string city = "Berlin";
            const string query = "Berlin, Germany";

            var userLocation = new UserLocationResponse
            {
                Latitude = latitude,
                Longitude = longitude,
                Country = country,
                City = city,
                Query = query
            };

            Assert.AreEqual(latitude, userLocation.Latitude);
            Assert.AreEqual(longitude, userLocation.Longitude);
            Assert.AreEqual(country, userLocation.Country);
            Assert.AreEqual(city, userLocation.City);
            Assert.AreEqual(query, userLocation.Query);
        }

        [TestMethod]
        public void UserLocationResponse_Constructor_ShouldHandleNullValues()
        {
            const double latitude = 0;
            const double longitude = 0;
            string? country = null;
            string? city = null;
            string? query = null;

            var userLocation = new UserLocationResponse
            {
                Latitude = latitude,
                Longitude = longitude,
                Country = country,
                City = city,
                Query = query
            };

            Assert.AreEqual(latitude, userLocation.Latitude);
            Assert.AreEqual(longitude, userLocation.Longitude);
            Assert.IsNull(userLocation.Country);
            Assert.IsNull(userLocation.City);
            Assert.IsNull(userLocation.Query);
        }
    }
}
