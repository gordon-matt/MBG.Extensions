
namespace MBG.Extensions.Xml
{
    public static class StringExtensions
    {
        public static string WrapInCDATA(this string s)
        {
            return string.Concat("<![CDATA[", s, "]]>");
        }
    }
}