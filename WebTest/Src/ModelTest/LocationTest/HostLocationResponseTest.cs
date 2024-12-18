using Web.Src.Model.Location;

namespace WebTest.Src.ModelTest.LocationTest
{
    [TestClass]
    public class HostLocationResponseTest
    {
        [TestMethod]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            const double latitude = 52.5200;
            const double longitude = 13.4050;
            const string? country = "Germany";
            const string? city = "Berlin";

            var locationResponse = new HostLocationResponse
            {
                Latitude = latitude,
                Longitude = longitude,
                Country = country,
                City = city
            };

            Assert.AreEqual(latitude, locationResponse.Latitude);
            Assert.AreEqual(longitude, locationResponse.Longitude);
            Assert.AreEqual(country, locationResponse.Country);
            Assert.AreEqual(city, locationResponse.City);
        }

        [TestMethod]
        public void Latitude_ShouldBeSetCorrectly()
        {
            const double latitude = 52.5200;

            var locationResponse = new HostLocationResponse
            {
                Latitude = latitude
            };

            Assert.AreEqual(latitude, locationResponse.Latitude);
        }

        [TestMethod]
        public void Longitude_ShouldBeSetCorrectly()
        {
            const double longitude = 13.4050;

            var locationResponse = new HostLocationResponse
            {
                Longitude = longitude
            };

            Assert.AreEqual(longitude, locationResponse.Longitude);
        }

        [TestMethod]
        public void Country_ShouldBeNullable()
        {
            string? country = null;

            var locationResponse = new HostLocationResponse
            {
                Country = country
            };

            Assert.IsNull(locationResponse.Country);
        }

        [TestMethod]
        public void City_ShouldBeNullable()
        {
            string? city = null;

            var locationResponse = new HostLocationResponse
            {
                City = city
            };

            Assert.IsNull(locationResponse.City);
        }
    }
}
