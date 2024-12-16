using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;
using Web.Src.Model;

namespace Web.Src.Service
{
    public class SpeedTestService(IConfiguration configuration, 
        ILocationService locationService) : ISpeedTestService
    {
        private static readonly int[] DownloadSizes = [350, 750, 1500, 3000];
        private const int Buffer = 8192;
        private const double MegabyteSize = 1024;
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
                result.Append("Physical Address: " + adapter.GetPhysicalAddress() + "\n");
                result.Append("Operational status: " + adapter.OperationalStatus + "\n");

            }

            return "Number of interfaces: " + nics.Length + 
                   "\nHost Name " + computerProperties.HostName + "\n" +
                   result;
        }

        public async Task<double> FastDownloadSpeedAsync(TimeSpan duration)
        {
            var url = configuration["SpeedTest:DownloadUrl"];
            if (string.IsNullOrEmpty(url))
            {
                throw new InvalidOperationException("Download URL isn't configured");
            }

            try
            {
                return await MeasureDownloadSpeedFromUrlAsync(url, duration);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error measuring fast download speed: {ex.Message}");
                throw;
            }
        }

        public async Task<DownloadSpeed> GetDownloadSpeed(string? host = null)
        {
            try
            {
                Server? server;
                if (host != null)
                {
                    var servers = await locationService.LoadServersAsync();
                    server = servers.FirstOrDefault(s => 
                        s.Host.Equals(host, StringComparison.OrdinalIgnoreCase));
                    if (server == null)
                    {
                        throw new Exception($"Server with host '{host}' not found in the server list");
                    }
                }
                else
                {
                    server = await locationService.GetBestServerAsync();
                }

                var pingService = new PingService();
                var ping = await pingService.CheckPingAsync(server!.Host);

                var downloadUrls = GenerateDownloadUrls(server, 3);

                const int testCount = 3;
                var speeds = new List<double>();

                for (var i = 0; i < testCount; i++)
                {
                    var speed = await MeasureDownloadSpeedAsync(downloadUrls);
                    speeds.Add(speed);
                }

                var speedAverage = speeds.Count > 0 ? speeds.Average() : 0;

                return new DownloadSpeed
                {
                    Server = server,
                    Speed = Math.Round(speedAverage, 3),
                    Unit = "Mbps",
                    Ping = ping,
                    Source = "SpeedTestService"
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get download speed", ex);
            }
        }

        private static IEnumerable<string> GenerateDownloadUrls(Server server, int retryCount = 1)
        {
            var downloadUriBase = new Uri(
                new Uri($"http://{server.Host}:8080/"), ".").OriginalString + 
                                  "random{0}x{0}.jpg?r={1}";

            foreach (var downloadSize in DownloadSizes)
            {
                for (var i = 0; i < retryCount; i++)
                {
                    yield return string.Format(downloadUriBase, downloadSize, i + 1);
                }
            }
        }

        private static async Task<double> MeasureDownloadSpeedAsync(IEnumerable<string> downloadUrls)
        {
            double totalSpeed = 0;
            var count = 0;

            foreach (var url in downloadUrls)
            {
                try
                {
                    var speed = await MeasureDownloadSpeedFromUrlAsync(url, TimeSpan.FromSeconds(5));
                    totalSpeed += speed;
                    count++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error downloading file from URL {url}: {ex.Message}");
                }
            }

            return count > 0 ? Math.Round(totalSpeed / count, 3) : 0;
        }

        private static async Task<double> MeasureDownloadSpeedFromUrlAsync(string url, TimeSpan timeout)
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = timeout;

            var stopwatch = Stopwatch.StartNew();

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                "(KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to download file. HTTP Status: {response.StatusCode}");
            }

            var contentStream = await response.Content.ReadAsStreamAsync();
            var buffer = new byte[Buffer];
            long totalBytesRead = 0;
            int bytesRead;

            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                totalBytesRead += bytesRead;
            }

            stopwatch.Stop();
            var timeInSeconds = stopwatch.Elapsed.TotalSeconds;
            var speedInMbps = ((totalBytesRead / MegabyteSize / MegabyteSize) / timeInSeconds) * 8;
            return Math.Round(speedInMbps, 3);
        }
    }
}