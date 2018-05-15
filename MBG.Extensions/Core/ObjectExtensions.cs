using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using MBG.Extensions.Reflection;

namespace MBG.Extensions.Core
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Serializes the specified System.Object and returns the data.
        /// </summary>
        /// <typeparam name="T">This item's type</typeparam>
        /// <param name="item">This item</param>
        /// <returns>Serialized data of specified System.Object as System.Byte[]</returns>
        public static byte[] BinarySerialize<T>(this T item) where T : ISerializable
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, item);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Serializes the specified System.Object and writes the data to the specified file.
        /// </summary>
        /// <typeparam name="T">This item's type</typeparam>
        /// <param name="item">This item</param>
        /// <param name="fileName">The name of the file to save the serialized data to.</param>
        public static void BinarySerialize<T>(this T item, string fileName) where T : ISerializable
        {
            using (Stream stream = File.Open(fileName, FileMode.Create))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, item);
                stream.Close();
            }
        }

        /// <summary>
        /// Creates a deep clone of the current System.Object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The original object.</param>
        /// <returns>A clone of the original object</returns>
        public static T DeepClone<T>(this T item) where T : ISerializable
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, item);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }

        /// <summary>
        /// Determines whether this System.Object is contained in the specified IEnumerable
        /// </summary>
        /// <param name="o">The System.Object</param>
        /// <param name="enumerable">The IEnumerable to check</param>
        /// <returns>true if enumerable contains this System.Object, otherwise false.</returns>
        public static bool In(this object o, IEnumerable enumerable)
        {
            foreach (object item in enumerable)
            {
                if (item.Equals(o))
                { return true; }
            }
            return false;
        }

        /// <summary>
        /// Determines whether this T is contained in the specified 'IEnumerable of T'
        /// </summary>
        /// <typeparam name="T">This System.Object's type</typeparam>
        /// <param name="t">This item</param>
        /// <param name="enumerable">The 'IEnumerable of T' to check</param>
        /// <returns>true if enumerable contains this item, otherwise false.</returns>
        public static bool In<T>(this T t, IEnumerable<T> enumerable)
        {
            foreach (T item in enumerable)
            {
                if (item.Equals(t))
                { return true; }
            }
            return false;
        }

        /// <summary>
        /// Determines whether this System.Object is contained in the specified values
        /// </summary>
        /// <param name="o">The System.Object</param>
        /// <param name="items">The values to compare</param>
        /// <returns>true if values contains this System.Object, otherwise false.</returns>
        public static bool In(this object o, params object[] items)
        {
            foreach (object item in items)
            {
                if (item.Equals(o))
                { return true; }
            }
            return false;
        }

        /// <summary>
        /// Determines whether this T is contained in the specified values
        /// </summary>
        /// <typeparam name="T">This System.Object's type</typeparam>
        /// <param name="t">This item</param>
        /// <param name="items">The values to compare</param>
        /// <returns>true if values contains this item, otherwise false.</returns>
        public static bool In<T>(this T t, params T[] items)
        {
            foreach (T item in items)
            {
                if (item.Equals(t))
                { return true; }
            }
            return false;
        }

        /// <summary>
        /// <para>Serializes the specified System.Object and writes the XML document</para>
        /// <para>to the specified file.</para>
        /// </summary>
        /// <typeparam name="T">This item's type</typeparam>
        /// <param name="item">This item</param>
        /// <param name="fileName">The file to which you want to write.</param>
        /// <returns>true if successful, otherwise false.</returns>
        public static bool XmlSerialize<T>(this T item, string fileName)
        {
            return item.XmlSerialize(fileName, true);
        }

        /// <summary>
        /// <para>Serializes the specified System.Object and writes the XML document</para>
        /// <para>to the specified file.</para>
        /// </summary>
        /// <typeparam name="T">This item's type</typeparam>
        /// <param name="item">This item</param>
        /// <param name="fileName">The file to which you want to write.</param>
        /// <param name="removeNamespaces">
        ///     <para>Specify whether to remove xml namespaces.</para>para>
        ///     <para>If your object has any XmlInclude attributes, then set this to false</para>
        /// </param>
        /// <returns>true if successful, otherwise false.</returns>
        public static bool XmlSerialize<T>(this T item, string fileName, bool removeNamespaces)
        {
            object locker = new object();

            XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();
            xmlns.Add(string.Empty, string.Empty);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            lock (locker)
            {
                using (XmlWriter writer = XmlWriter.Create(fileName, settings))
                {
                    if (removeNamespaces)
                    {
                        xmlSerializer.Serialize(writer, item, xmlns);
                    }
                    else { xmlSerializer.Serialize(writer, item); }

                    writer.Close();
                }
            }

            return true;
        }

        /// <summary>
        /// Serializes the specified System.Object and returns the serialized XML
        /// </summary>
        /// <typeparam name="T">This item's type</typeparam>
        /// <param name="item">This item</param>
        /// <returns>Serialized XML for specified System.Object</returns>
        public static string XmlSerialize<T>(this T item)
        {
            return item.XmlSerialize(true);
        }

        /// <summary>
        /// Serializes the specified System.Object and returns the serialized XML
        /// </summary>
        /// <typeparam name="T">This item's type</typeparam>
        /// <param name="item">This item</param>
        /// <param name="removeNamespaces">
        ///     <para>Specify whether to remove xml namespaces.</para>para>
        ///     <para>If your object has any XmlInclude attributes, then set this to false</para>
        /// </param>
        /// <returns>Serialized XML for specified System.Object</returns>
        public static string XmlSerialize<T>(this T item, bool removeNamespaces)
        {
            object locker = new object();

            XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();
            xmlns.Add(string.Empty, string.Empty);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            lock (locker)
            {
                StringBuilder stringBuilder = new StringBuilder();
                using (StringWriter stringWriter = new StringWriter(stringBuilder))
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings))
                    {
                        if (removeNamespaces)
                        {
                            xmlSerializer.Serialize(xmlWriter, item, xmlns);
                        }
                        else { xmlSerializer.Serialize(xmlWriter, item); }

                        return stringBuilder.ToString();
                    }
                }
            }
        }
    }
}