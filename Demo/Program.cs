using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MBG.Extensions.Core;
using System.IO;
using MBG.Extensions.IO;
using System.Data;
using MBG.Extensions.Data;

namespace Demo
{
    class Program
    {
        public enum MyEnum
        {
            MyValue1,
            MyValue2,
            MyValue3,
            MyValue4
        }
        static void Main(string[] args)
        {
            //Dictionary<string, string> dic = new Dictionary<string, string>();
            //dic.Add("1", "dude");
            //dic.Add("15", "this");
            //dic.Add("6", "is cool");
            //byte[] bytes = dic.BinarySerialize();
            //Dictionary<string, string> dic2 = bytes.BinaryDeserialize<Dictionary<string, string>>();

            //Here is a demo of some of the most useful extensions:

            #region Demo use for In() method:
            MyEnum myEnum = MyEnum.MyValue1;

            if (myEnum.In(MyEnum.MyValue1, MyEnum.MyValue4))
            {
                //Do something
            }

            string myString = "Ta Da!";
            if (myString.In("voila", "viva", "ba-da-bing!"))
            {
                //Do something
            }

            List<int> values = new List<int>();
            values.Add(1);
            values.Add(2);
            values.Add(3);
            int myInt = 45;
            // Ienumerable<T> version of In()
            if (myInt.In(values))
            {

            }
            #endregion

            //Old way
            string myRepetitiveTraditional = "============================================================";

            //new way
            string myNewRepetitve = '='.Repeat(50);

            #region Demo use for XmlSerialize / Deserialize methods

            //TIP: Check out the PeopleXML class for a demo of how to easily implement
            //Load and Save methods for your custom objects!!

            PeopleXML people = GetDemoObject();

            people.XmlSerialize("C:\\xmlserliazeDemo.xml");

            FileInfo fileInfo = new FileInfo("C:\\xmlserliazeDemo.xml");
            PeopleXML people2 = fileInfo.XmlDeserialize<PeopleXML>();
            people2.People.ForEach(WritePersonToConsole);

            //or:
            string xml = people.XmlSerialize();
            PeopleXML pc = xml.XmlDeserialize<PeopleXML>();

            #endregion

            #region demo IsMultipleOf()

            //traditional:
            int i = 23;
            if (i % 10 == 0)
            {
            }

            //new:
            if (i.IsMultipleOf(10)) //so much cleaner!!
            {
            }
            #endregion

            #region Demo DataColumnCollection.AddRange

            DataTable table = new DataTable();

            //Old way
            table.Columns.Add("col1");
            table.Columns.Add("col2");
            table.Columns.Add("col3");

            //or
            table.Columns.Add(new DataColumn("col4", typeof(string)));
            table.Columns.Add(new DataColumn("col5", typeof(bool)));
            table.Columns.Add(new DataColumn("col6", typeof(int)));

            //new way
            table.Columns.AddRange("col7", "col8", "col9");

            //or
            table.Columns.AddRange(
                new DataColumn("col10", typeof(string)),
                new DataColumn("col11", typeof(bool)),
                new DataColumn("col12", typeof(int)));


            #endregion

            #region ToCSV demo

            DataTable tableForCSV = GetDataTableForDemo();
            tableForCSV.ToCsv("C:\\csvDemo.csv");

            #endregion
        }

        static void WritePersonToConsole(Person p)
        {
            Console.WriteLine(string.Format("Name: {0}{1}Age: {2}{1}Mother:{3}{1}Father:{4}",
                p.Name, Environment.NewLine, p.Age, p.Mother ?? string.Empty, p.Father ?? string.Empty));
        }

        static PeopleXML GetDemoObject()
        {
            PeopleXML peopleXML = new PeopleXML();
            PersonCollection people = new PersonCollection();
            Person john = new Person();
            john.Name = "John";
            john.Age = 29;

            people.Add(john);

            Person susan = new Person();
            susan.Name = "Susan";
            susan.Age = 25;

            people.Add(susan);

            Person luke = new Person();
            luke.Name = "Luke";
            luke.Age = 2;
            luke.Mother = susan.Name;
            luke.Father = john.Name;

            people.Add(luke);

            peopleXML.People = people;
            return peopleXML;
        }

        static DataTable GetDataTableForDemo()
        {
            DataTable table = new DataTable();
            table.Columns.AddRange("col1", "col2", "col3");

            DataRow row = table.NewRow();
            row["col1"] = "row1col1";
            row["col2"] = "row1col2";
            row["col3"] = "row1col3";
            table.Rows.Add(row);

            row = table.NewRow();
            row["col1"] = "row2col1";
            row["col2"] = "row2col2";
            row["col3"] = "row2col3";
            table.Rows.Add(row);

            row = table.NewRow();
            row["col1"] = "row3col1";
            row["col2"] = "row3col2";
            row["col3"] = "row3col3";
            table.Rows.Add(row);

            return table;
        }
    }
}
