using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace MBG.Extensions.Xml
{
    public static class XmlNodeExtensions
    {
        public static XElement ToXElement(this XmlNode node)
        {
            return XElement.Parse(node.ToString());
        }
        public static XmlNode GetChildNodeByName(this XmlNode node, string name)
        {
            return GetChildNodesByName(node, name).FirstOrDefault();
        }
        public static IEnumerable<XmlNode> GetChildNodesByName(this XmlNode node, string name)
        {
            return (from x in node.Cast<XmlNode>()
                    where x.Name == name
                    select x);
        }
    }
}