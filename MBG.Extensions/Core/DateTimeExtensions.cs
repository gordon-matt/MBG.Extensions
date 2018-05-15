using System;

namespace MBG.Extensions.Core
{
    public static class DateTimeExtensions
    {
        public static string ToISO8601DateString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff");
        }
    }
}