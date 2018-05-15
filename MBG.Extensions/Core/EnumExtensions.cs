using System;

namespace MBG.Extensions.Core
{
    public static class EnumExtensions
    {
        // Thanks to the guys from http://signum.codeplex.com/
        public static T ToEnum<T>(this string str) where T : struct
        {
            return (T)Enum.Parse(typeof(T), str);
        }
        // Thanks to the guys from http://signum.codeplex.com/
        public static T ToEnum<T>(this string str, bool ignoreCase) where T : struct
        {
            return (T)Enum.Parse(typeof(T), str, ignoreCase);
        }
        // Thanks to the guys from http://signum.codeplex.com/
        public static T[] GetValues<T>()
        {
            return (T[])Enum.GetValues(typeof(T));
        }
        public static T Parse<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }
    }
}