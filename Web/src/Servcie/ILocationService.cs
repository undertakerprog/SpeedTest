using Web.src.Model;

namespace Web.src.Servcie
{
    public interface ILocationService
    {
        Task<(double Latitude, double Longitude, string Country, string City, string Query)> GetUserLocationAsync();
        Task<(double Latitude, double Longtitude, string Country, string City)> GetLocationByIpAsync(string ipAddress);
        Task<List<Server>> GetServersByCityAsync(string city);
        Task<Server?> GetClosestServerAsync();
        Task<Server?> GetBestServerAsync();
        Task<List<Server>> LoadServersAsync();
    }
}
