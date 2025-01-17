namespace Desktop.Helpers
{
    public static class SpeedConverter
    {
        public static double ConvertSpeed(long bytesDelta, string unit)
        {
            var bitsDelta = bytesDelta * 8;
            return unit switch
            {
                "KBps" => bytesDelta / 1000.0,
                "MBps" => bytesDelta / 1000000.0,
                "Kbps" => bitsDelta / 1000.0,
                "Mbps" => bitsDelta / 1000000.0,
                _ => bitsDelta / 1000000.0
            };
        }
    }
}
