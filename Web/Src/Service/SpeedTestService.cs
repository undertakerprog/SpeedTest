using System.Diagnostics;
using Web.Src.Model;

namespace Web.Src.Service
{
    public class SpeedTestService(
        IConfiguration configuration,
        ILocationService locationService,
        Func<HttpClient> httpClientFactory)
        : ISpeedTestService
    {
        private static readonly int[] DownloadSizes = [350, 750, 1500, 3000];
        private const int Buffer = 8192;
        private const double MegabyteSize = 1024;
        private readonly TimeSpan _downloadTimeout = TimeSpan.FromSeconds(5);

        public async Task<double> FastDownloadSpeedAsync(TimeSpan duration)
        {
            var url = configuration["SpeedTest:DownloadUrl"]
                      ?? throw new InvalidOperationException("Download URL isn't configured");

            double totalDownloaded = 0;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                using var cts = new CancellationTokenSource();
                cts.CancelAfter(duration);

                var downloadedBytes = await DownloadFileAsync(url, cts.Token);
                totalDownloaded += downloadedBytes;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Download operation was canceled due to timeout.");
            }
            catch (Exception ex)
            {
                throw new Exception("Speed ​​measurement error", ex);
            }
            finally
            {
                stopwatch.Stop();
            }

            if (stopwatch.Elapsed.TotalSeconds == 0 || totalDownloaded == 0)
            {
                return 0;
            }

            var speedInMbps = ((totalDownloaded / MegabyteSize / MegabyteSize) / stopwatch.Elapsed.TotalSeconds) * 8;
            return Math.Round(speedInMbps, 3);
        }

        public async Task<DownloadSpeed> GetDownloadSpeed(string? host = null)
        {
            try
            {
                var server = host != null
                    ? (await locationService.LoadServersAsync())
                      .FirstOrDefault(s => s.Host.Equals(host, StringComparison.OrdinalIgnoreCase))
                      ?? throw new Exception($"Server '{host}' not found")
                    : await locationService.GetBestServerAsync();

                var pingService = new PingService();
                var ping = await pingService.CheckPingAsync(server!.Host, 5000);

                var downloadUrls = GenerateDownloadUrls(server, 3);
                var speed = await DownloadAndMeasureSpeedAsync(downloadUrls, _downloadTimeout);

                return new DownloadSpeed
                {
                    Server = server,
                    Speed = Math.Round(speed, 3),
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

        private async Task<double> DownloadAndMeasureSpeedAsync(IEnumerable<string> urls, TimeSpan timeout)
        {
            double totalDownloaded = 0;
            var stopwatch = Stopwatch.StartNew();

            using var cts = new CancellationTokenSource(timeout);

            while (stopwatch.Elapsed < timeout)
            {
                foreach (var url in urls)
                {
                    try
                    {
                        totalDownloaded += await DownloadFileAsync(url, cts.Token);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка загрузки {url}: {ex.Message}");
                    }
                }
            }

            stopwatch.Stop();
            var speedInMbps = ((totalDownloaded / MegabyteSize / MegabyteSize) / timeout.TotalSeconds) * 8;
            return Math.Round(speedInMbps, 3);
        }

        private async Task<long> DownloadFileAsync(string url, CancellationToken cancellationToken)
        {
            long totalBytesRead = 0;

            using var httpClient = httpClientFactory();
            httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                                              "(KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

            try
            {
                using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error download. HTTP status: {response.StatusCode}");
                }

                await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                var buffer = new byte[Buffer];

                int bytesRead;
                while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                {
                    totalBytesRead += bytesRead;
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                throw new Exception("Error downloading file", ex);
            }

            return totalBytesRead;
        }
    }
}