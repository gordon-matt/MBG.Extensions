using System.Net;
using System.Net.NetworkInformation;

namespace MBG.Extensions.Net
{
    public static class IPAddressExtensions
    {
        public static IPStatus Ping(this IPAddress ipAddress)
        {
            return ipAddress.Ping(3000);
        }
        public static IPStatus Ping(this IPAddress ipAddress, int timeout)
        {
            return new Ping().Send(ipAddress, timeout).Status;
        }
    }
}