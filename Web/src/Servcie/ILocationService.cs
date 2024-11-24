namespace Web.src.Servcie
{
    public interface ILocationService
    {
        Task<(double Latitude, double Longitude, string Country, string City, string Query)> GetUserLocationAsync();
        Task<(double Latitude, double Longtitude, string Country)> GetLocationByIPAsync(string ipAdress);
    }
}
