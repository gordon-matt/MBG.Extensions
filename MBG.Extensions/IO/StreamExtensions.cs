using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;

namespace MBG.Extensions.IO
{
    public static class StreamExtensions
    {
        public static T BinaryDeserialize<T>(this Stream stream)
        {
            return (T)new BinaryFormatter().Deserialize(stream);
        }
        public static void BinarySerialize<T>(this Stream stream, T item)
        {
            new BinaryFormatter().Serialize(stream, item);
        }

        //NOTE: .NET 4.0 already has this extension built in. Don't need this when
        //targeting .NET 4.0 application; only for .NET 3.5
        public static void CopyTo(this Stream origin, Stream destination)
        {
            origin.CopyTo(destination, 0x1000); //4096
        }
        public static void CopyTo(this Stream origin, Stream destination, int bufferSize)
        {
            int bytesRead;
            byte[] buffer = new byte[bufferSize];
            while ((bytesRead = origin.Read(buffer, 0, buffer.Length)) != 0)
            {
                destination.Write(buffer, 0, bytesRead);
            }
            //destination.Flush(); //Needed?
        }

        public static MemoryStream DeflateCompress(this Stream stream)
        {
            MemoryStream memoryStream = new MemoryStream();
            using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
            {
                stream.CopyTo(deflateStream);
            }
            return memoryStream;
        }
        public static MemoryStream DeflateDecompress(this Stream stream)
        {
            MemoryStream memoryStream = new MemoryStream();
            using (DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress))
            {
                deflateStream.CopyTo(memoryStream);
            }
            return memoryStream;
        }
        public static MemoryStream GZipCompress(this Stream stream)
        {
            MemoryStream memoryStream = new MemoryStream();
            using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress))
            {
                stream.CopyTo(gZipStream);
            }
            return memoryStream;
        }
        public static MemoryStream GZipDecompress(this Stream stream)
        {
            MemoryStream memoryStream = new MemoryStream();
            using (GZipStream gZipStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                gZipStream.CopyTo(memoryStream);
            }
            return memoryStream;
        }
    }
}