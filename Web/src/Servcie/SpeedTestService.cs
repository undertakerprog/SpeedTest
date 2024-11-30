using System.Net.NetworkInformation;
using System.Text;

namespace Web.src.Servcie
{
    public class SpeedTestService : ISpeedTestService
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
            return "Number of interfaces: " + nics.Length + "\nHost Name " + computerProperties.HostName + "\n" + result;
        }
    }
}