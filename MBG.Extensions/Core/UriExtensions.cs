using System;
using System.Net.NetworkInformation;

namespace MBG.Extensions.Core
{
    public static class UriExtensions
    {
        //public static bool Ping(this Uri uri)
        //{
        //    return uri.Ping(3000);
        //}
        //public static bool Ping(this Uri uri, int timeout)
        //{
        //    PingReply reply = new Ping().Send(uri.Host, timeout);
        //    return reply.Status == IPStatus.Success;
        //}

        public static IPStatus Ping(this Uri uri)
        {
            return uri.Ping(3000);
        }
        public static IPStatus Ping(this Uri uri, int timeout)
        {
            return new Ping().Send(uri.Host, timeout).Status;
        }
    }
}