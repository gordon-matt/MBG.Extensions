using System.Linq;
using System.Xml;

namespace MBG.Extensions.Xml
{
    public static class XmlAttributeCollectionExtensions
    {
        public static bool Exists(this XmlAttributeCollection attributes, string attributeName)
        {
            return attributes.Cast<XmlAttribute>().Any(x => x.Name == attributeName);
        }

        /// <summary>
        /// Returns the value if the value is not null or empty. Otherwise, returns specified default value
        /// </summary>
        /// <param name="attributes">The XmlAttributeCollection</param>
        /// <param name="attributeName">The name of the attribute to fetch</param>
        /// <param name="specifiedDefault">The default value to return if the attribute's value is null or empty</param>
        /// <returns></returns>
        public static string GetValueOrSpecified(this XmlAttributeCollection attributes, string attributeName, string specifiedDefault)
        {
            if (attributes.Exists(attributeName) &&
                !string.IsNullOrEmpty(attributes[attributeName].Value))
            {
                return attributes[attributeName].Value;
            }
            return specifiedDefault;
        }
    }
}