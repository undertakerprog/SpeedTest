using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Desktop.Helpers;
using Desktop.Models;

namespace Desktop.Services
{
    public class SpeedTestService(HttpClient httpClient)
    {
        public async Task<string> GetFastDownloadSpeedAsync()
        {
            try
            {
                var response = await httpClient.GetAsync("/api/SpeedTest/fast-test");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<DownloadSpeedResponse>(json);

                    if (result == null) return "Invalid response.";

                    var speedMbps = result.SpeedMbps;
                    var speedUnit = Properties.Settings.Default.SpeedUnit;

                    var convertedSpeed = SpeedConverter.ConvertMbpsToOtherUnits(speedMbps, speedUnit);

                    return $"{convertedSpeed:F2} {speedUnit}";

                }
                else
                {
                    return $"Server error: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}