using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using Desktop.Helpers;
using Desktop.Models;

namespace Desktop.Services
{
    public class DesktopSpeedTestService(HttpClient httpClient)
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

                    var speedMbps = result.Speed;
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

        public async Task<string> GetDownloadSpeedAsync(string host)
        {
            try
            {
                var response = await httpClient.GetAsync($"/api/SpeedTest/download-speed?host={host}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<DownloadSpeedResponse>(json);

                    if (result == null) return "Invalid response";

                    var speedMbps = result.Speed;
                    var ping = result.Ping;
                    var speedUnit = Properties.Settings.Default.SpeedUnit;

                    var convertedSpeed = SpeedConverter.ConvertMbpsToOtherUnits(speedMbps, speedUnit);

                    Debug.WriteLine($"Speed: {speedMbps}");
                    return $"Download Speed: {convertedSpeed:F2} {speedUnit}\nPing: {ping} ms";
                }
                return $"Server error: {response.StatusCode}";
            } 
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}