using Microsoft.Extensions.Primitives;
using System.Net.NetworkInformation;
using System.Text;

namespace Web.src.Servcie
{
    public class SpeedTestService : ISpeedTestService
    {
        public string GetSpeedTestResult()
        {
            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            if (nics == null || nics.Length < 1)
            {
                return "No network interfaces found";
            }

            StringBuilder result = new StringBuilder();

            foreach (NetworkInterface adapter in nics)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                result.Append("Name: " + adapter.Name + "\n");
                result.Append("Physical Adress: " + adapter.GetPhysicalAddress().ToString() + "\n");
                result.Append("Operational status: " + adapter.OperationalStatus + "\n");
                
            }
            return "Number of interfaces: " + nics.Length + "\nHost Name " + computerProperties.HostName + "\n" + result;
        }
    }
}
