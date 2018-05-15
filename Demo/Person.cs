using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using MBG.Extensions.Core;
using MBG.Extensions.IO;
using System.IO;

namespace Demo
{
    [XmlRoot]
    [Serializable]
    public class PeopleXML
    {
        public PersonCollection People { get; set; }

        public PeopleXML()
        {
            People = new PersonCollection();
        }

        public static PeopleXML Load(string fileName)
        {
            return new FileInfo(fileName).XmlDeserialize<PeopleXML>();
        }
        public void Save(string fileName)
        {
            this.XmlSerialize(fileName);
        }
    }
    public class Person
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public byte Age { get; set; }
        [XmlAttribute]
        public string Mother { get; set; }
        [XmlAttribute]
        public string Father { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
    public class PersonCollection : List<Person>
    {
        public Person this[string name]
        {
            get { return this.SingleOrDefault(p => p.Name == name); }
        }
    }
}