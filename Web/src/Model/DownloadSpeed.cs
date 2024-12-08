namespace Web.src.Model
{
    public class DownloadSpeed
    {
        public Server Server { get; set; } = new Server();
        public double Speed { get; set; }
        public string Unit { get; set; } = "unknown";
        public string Source { get; set; } = "unknown";
    }
}
