using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;
using Web.src.Model;

namespace Web.src.Servcie
{
    public class SpeedTestService(IConfiguration configuration) : ISpeedTestService
    {
        public string GetInterface()
        {
            var computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            var nics = NetworkInterface.GetAllNetworkInterfaces();
            if (nics.Length < 1)
            {
                return "No network interfaces found";
            }

            var result = new StringBuilder();

            foreach (var adapter in nics)
            {
                var properties = adapter.GetIPProperties();
                result.Append("Name: " + adapter.Name + "\n");
                result.Append("Physical Address: " + adapter.GetPhysicalAddress().ToString() + "\n");
                result.Append("Operational status: " + adapter.OperationalStatus + "\n");

            }

            return "Number of interfaces: " + nics.Length + "\nHost Name " + computerProperties.HostName + "\n" +
                   result;
        }

        public async Task<double> MeasureDownloadSpeedAsync(TimeSpan duration)
        {
            var url = configuration["SpeedTest:DownloadUrl"];
            if (string.IsNullOrEmpty(url))
            {
                throw new InvalidOperationException("Download URL isn't configured");
            }

            using var httpClient = new HttpClient();
            httpClient.Timeout = duration;
            var stopwatch = Stopwatch.StartNew();

            var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Failed to download file. HTTP Status: {response.StatusCode}");
            }

            var contentStream = await response.Content.ReadAsStreamAsync();
            var buffer = new byte[8192];
            long totalBytesRead = 0;
            int bytesRead;

            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                totalBytesRead += bytesRead;
            }

            stopwatch.Stop();
            var timeInSeconds = stopwatch.Elapsed.TotalSeconds;
            var speedMbps = Math.Round(((totalBytesRead / 1024.0 / 1024.0) / timeInSeconds) * 8, 3);
            return speedMbps;
        }
    }
}