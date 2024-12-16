namespace Web.Src.Model
{
    public class DownloadSpeed
    {
        public Server Server { get; set; } = new();
        public double Speed { get; set; }
        public string Unit { get; set; } = "unknown";
        public double Ping { get; set; }
        public string Source { get; set; } = "unknown";
    }
}
