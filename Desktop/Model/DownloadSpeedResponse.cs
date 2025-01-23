using System.Text.Json.Serialization;

namespace Desktop.Models
{
    public class DownloadSpeedResponse
    {
        [JsonPropertyName("speed")]
        public double Speed { get; init; }
        [JsonPropertyName("ping")]
        public int Ping { get; init; }
    }
}
