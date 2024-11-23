namespace Web.src.Model
{
    public class UpdateHostRequest
    {
        public string Country { get; set; }
        public string NewHost { get; set; }
        public UpdateHostRequest()
        {
            Country = "unknown";
            NewHost = "unknown";
        }
    }
}
