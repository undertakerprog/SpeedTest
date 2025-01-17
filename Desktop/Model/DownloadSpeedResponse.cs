using System.Text.Json.Serialization;

namespace Desktop.Models
{
    public class DownloadSpeedResponse
    {
        [JsonPropertyName("speedMbps")]
        public double SpeedMbps { get; init; }
    }
}
