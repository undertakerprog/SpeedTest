using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;
using Web.src.Model;

namespace Web.src.Servcie
{
    public class SpeedTestService(IConfiguration configuration, 
        HttpClient httpClient, ILocationService locationService) : ISpeedTestService
    {
        private static readonly int[] DownloadSizes = [350, 3000];
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

            return "Number of interfaces: " + nics.Length + "\nHost Name " + computerProperties.HostName + "\n" +
                   result;
        }

        public async Task<DownloadSpeed> GetDownloadSpeed()
        {
            try
            {
                var server = await locationService.GetClosestServerAsync();
                if (server == null)
                {
                    throw new Exception("No server found for testing");
                }

                var downloadUrls = GenerateDownloadUrls(server, 3);
                var speed = await MeasureDownloadSpeedAsync(downloadUrls);

                return new DownloadSpeed
                {
                    Server = server,
                    Speed = speed,
                    Unit = "Mbps",
                    Source = "SpeedTestService"
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get download speed", ex);
            }
        }

        public async Task<double> FastDownloadSpeedAsync(TimeSpan duration)
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

        private async Task<double> MeasureDownloadSpeedAsync(IEnumerable<string> downloadUrls)
        {
            double totalSpeed = 0;
            var count = 0;
            const int retryCount = 3;

            foreach (var url in downloadUrls)
            {
                var retries = 0;
                var success = false;
                while (retries < retryCount && !success)
                {
                    try
                    {
                        var stopwatch = Stopwatch.StartNew();

                        var request = new HttpRequestMessage(HttpMethod.Get, url);
                        request.Headers.Add("User-Agent", 
                            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                            "(KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

                        var response = await httpClient.SendAsync(request);

                        if (response.IsSuccessStatusCode)
                        {
                            var contentStream = await response.Content.ReadAsStreamAsync();
                            var buffer = new byte[8192];
                            long totalBytes = 0;
                            int bytesRead;

                            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                totalBytes += bytesRead;
                            }

                            stopwatch.Stop();
                            var timeTaken = stopwatch.Elapsed.TotalSeconds;
                            Console.WriteLine($"Time taken for {url}: {timeTaken} seconds");

                            var fileSizeInMB = totalBytes / (1024.0 * 1024.0);
                            Console.WriteLine($"Downloaded {fileSizeInMB} MB");

                            var speed = (fileSizeInMB * 8) / timeTaken;
                            Console.WriteLine($"Speed: {speed} Mbps");

                            totalSpeed += speed;
                            count++;
                            success = true;
                        }
                        else
                        {
                            Console.WriteLine($"Received non-success status code: {response.StatusCode} for URL: {url}. Retrying...");
                            retries++;
                            await Task.Delay(1000);
                        }
                    }
                    catch (Exception ex)
                    {
                        retries++;
                        Console.WriteLine($"Error downloading from URL {url}: {ex.Message}. Retrying...");
                        await Task.Delay(1000);
                    }
                }

                if (!success)
                {
                    Console.WriteLine($"Failed to download after {retryCount} attempts for URL: {url}");
                }
            }

            return count > 0 ? Math.Round(totalSpeed / count, 3) : 0;
        }
    }
}