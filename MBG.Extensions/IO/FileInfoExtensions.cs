using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using MBG.Extensions.Core;

namespace MBG.Extensions.IO
{
    public static class FileInfoExtensions
    {
        /// <summary>
        /// Deserializes the Binary data contained in the specified file.
        /// </summary>
        /// <typeparam name="T">The type of System.Object to be deserialized.</typeparam>
        /// <param name="fileInfo">This System.IO.FileInfo instance.</param>
        /// <returns>The System.Object being deserialized.</returns>
        public static T BinaryDeserialize<T>(this FileInfo fileInfo) where T : ISerializable
        {
            using (Stream stream = File.Open(fileInfo.FullName, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                T item = (T)binaryFormatter.Deserialize(stream);
                stream.Close();
                return item;
            }
        }

        /// <summary>
        /// Compresses the file using the Deflate algorithm and returns the name of the compressed file.
        /// </summary>
        /// <param name="fileInfo">This System.IO.FileInfo instance.</param>
        /// <returns>The name of the new compressed file.</returns>
        public static void DeflateCompress(this FileInfo fileInfo)
        {
            using (FileStream fsIn = fileInfo.OpenRead())
            {
                if (fileInfo.Extension != ".cmp")
                {
                    using (FileStream fsOut = File.Create(fileInfo.FullName + ".cmp"))
                    {
                        using (DeflateStream deflateStream = new DeflateStream(fsOut, CompressionMode.Compress))
                        {
                            fsIn.CopyTo(deflateStream);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Decompresses the file using the Deflate algorithm and returns the name of the decompressed file.
        /// </summary>
        /// <param name="fileInfo">This System.IO.FileInfo instance.</param>
        /// <returns>The name of the new decompressed file.</returns>
        public static void DeflateDecompress(this FileInfo fileInfo)
        {
            using (FileStream fsIn = fileInfo.OpenRead())
            {
                string originalName = fileInfo.FullName.Remove(fileInfo.FullName.Length - fileInfo.Extension.Length);

                using (FileStream fsOut = File.Create(originalName))
                {
                    using (DeflateStream deflateStream = new DeflateStream(fsIn, CompressionMode.Decompress))
                    {
                        deflateStream.CopyTo(fsOut);
                    }
                }
            }
        }

        public static byte[] GetBytes(this FileInfo fileInfo)
        {
            return fileInfo.GetBytes(0x1000);
        }

        /// <summary>
        /// Gets the file data as an array of bytes
        /// </summary>
        /// <param name="fileInfo">This System.IO.FileInfo instance.</param>
        /// <param name="maxBufferSize">The buffer size.</param>
        /// <returns>System.Byte[] representing the file data.</returns>
        public static byte[] GetBytes(this FileInfo fileInfo, long maxBufferSize)
        {
            byte[] buffer = new byte[(fileInfo.Length > maxBufferSize ? maxBufferSize : fileInfo.Length)];
            using (FileStream fileStream = new FileStream(
                fileInfo.FullName,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                buffer.Length))
            {
                fileStream.Read(buffer, 0, buffer.Length);
                fileStream.Close();
            }
            return buffer;
        }

        /// <summary>
        /// Gets the file size in KiloBytes
        /// </summary>
        /// <param name="fileInfo">This System.IO.FileInfo instance.</param>
        /// <returns>System.Double representing the size of the file.</returns>
        public static double GetFileSizeInKiloBytes(this FileInfo fileInfo)
        {
            return (double)fileInfo.Length / 1024;
        }

        /// <summary>
        /// Gets the file size in MegaBytes
        /// </summary>
        /// <param name="fileInfo">This System.IO.FileInfo instance.</param>
        /// <returns>System.Double representing the size of the file.</returns>
        public static double GetFileSizeInMegaBytes(this FileInfo fileInfo)
        {
            return fileInfo.GetFileSizeInKiloBytes() / 1024;
        }

        /// <summary>
        /// Gets the file size in GigaBytes
        /// </summary>
        /// <param name="fileInfo">This System.IO.FileInfo instance.</param>
        /// <returns>System.Double representing the size of the file.</returns>
        public static double GetFileSizeInGigaBytes(this FileInfo fileInfo)
        {
            return fileInfo.GetFileSizeInMegaBytes() / 1024;
        }

        /// <summary>
        /// Compresses the file using the GZip algorithm and returns the name of the compressed file.
        /// </summary>
        /// <param name="fileInfo">This System.IO.FileInfo instance.</param>
        /// <returns>The name of the new compressed file.</returns>
        public static void GZipCompress(this FileInfo fileInfo)
        {
            using (FileStream fsIn = fileInfo.OpenRead())
            {
                if (fileInfo.Extension != ".gz")
                {
                    using (FileStream fsOut = File.Create(fileInfo.FullName + ".gz"))
                    {
                        using (GZipStream gZipStream = new GZipStream(fsOut, CompressionMode.Compress))
                        {
                            fsIn.CopyTo(gZipStream);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Decompresses the file using the GZip algorithm and returns the name of the decompressed file.
        /// </summary>
        /// <param name="fileInfo">This System.IO.FileInfo instance.</param>
        /// <returns>The name of the new decompressed file.</returns>
        public static void GZipDecompress(this FileInfo fileInfo)
        {
            using (FileStream fsIn = fileInfo.OpenRead())
            {
                // Get original file extension, for example:
                // "doc" from report.doc.gz.
                string originalName = fileInfo.FullName.Remove(fileInfo.FullName.Length - fileInfo.Extension.Length);

                using (FileStream fsOut = File.Create(originalName))
                {
                    using (GZipStream gZipStream = new GZipStream(fsIn, CompressionMode.Decompress))
                    {
                        gZipStream.CopyTo(fsOut);
                    }
                }
            }
        }

        /// <summary>
        /// Encrypts the specified file and also returns that encrypted data as a System.Byte[].
        /// </summary>
        /// <param name="fileInfo">This System.IO.FileInfo instance.</param>
        /// <param name="encoding">The System.Text.Encoding to use.</param>
        /// <param name="key">The secret key to use for the symmetric algorithm.</param>
        /// <param name="initializationVector">The initialization vector to use for the symmetric algorithm.</param>
        /// <returns>Encrypted data as a System.Byte[].</returns>
        public static byte[] TripleDESEncrypt(this FileInfo fileInfo, Encoding encoding, byte[] key, byte[] initializationVector)
        {
            byte[] bytes = null;
            using (FileStream fsIn = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader streamReader = new StreamReader(fsIn))
                {
                    bytes = streamReader.ReadToEnd().TripleDESEncrypt(
                        encoding,
                        key,
                        initializationVector);
                }
            }
            using (FileStream fsOut = new FileStream(fileInfo.FullName, FileMode.Create, FileAccess.Write))
            {
                fsOut.Write(bytes, 0, bytes.Length);
            }
            return bytes;
        }

        /// <summary>
        /// Decrypts the specified file and also returns that decrypted data as a System.String.
        /// </summary>
        /// <param name="fileInfo">This System.IO.FileInfo instance.</param>
        /// <param name="encoding">The System.Text.Encoding to use.</param>
        /// <param name="key">The secret key to use for the symmetric algorithm.</param>
        /// <param name="initializationVector">The initialization vector to use for the symmetric algorithm.</param>
        /// <returns>Decrypted data as a System.String.</returns>
        public static string TripleDESDecrypt(this FileInfo fileInfo, Encoding encoding, byte[] key, byte[] initializationVector)
        {
            string decrypted = string.Empty;

            using (FileStream fsIn = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read))
            {
                using (CryptoStream cryptoStream = new CryptoStream(
                    fsIn,
                    new TripleDESCryptoServiceProvider().CreateDecryptor(key, initializationVector),
                    CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new StreamReader(cryptoStream))
                    {
                        decrypted = streamReader.ReadToEnd();
                    }
                }
            }

            using (FileStream fsOut = new FileStream(fileInfo.FullName, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter streamWriter = new StreamWriter(fsOut))
                {
                    streamWriter.Write(decrypted);
                    streamWriter.Flush();
                }
            }

            return decrypted;
        }

        //public static byte[] TripleDESEncrypt(this FileInfo fileInfo, Encoding encoding, byte[] key, byte[] initializationVector)
        //{
        //    using (FileStream fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read))
        //    {
        //        using (StreamReader streamReader = new StreamReader(fileStream))
        //        {
        //            return streamReader.ReadToEnd().TripleDESEncrypt(
        //                encoding,
        //                key,
        //                initializationVector);
        //        }
        //    }
        //}
        //public static string TripleDESDecrypt(this FileInfo fileInfo, Encoding encoding, byte[] key, byte[] initializationVector)
        //{
        //    using (FileStream fStream = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read))
        //    {
        //        using (CryptoStream cryptoStream = new CryptoStream(
        //            fStream,
        //            new TripleDESCryptoServiceProvider().CreateDecryptor(key, initializationVector),
        //            CryptoStreamMode.Read))
        //        {
        //            using (StreamReader streamReader = new StreamReader(cryptoStream))
        //            {
        //                return streamReader.ReadToEnd();
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Deserializes the XML data contained in the specified file.
        /// </summary>
        /// <typeparam name="T">The type of System.Object to be deserialized.</typeparam>
        /// <param name="fileInfo">This System.IO.FileInfo instance.</param>
        /// <returns>The System.Object being deserialized.</returns>
        public static T XmlDeserialize<T>(this FileInfo fileInfo)
        {
            string xml = string.Empty;
            using (FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    return sr.ReadToEnd().XmlDeserialize<T>();
                }
            }
        }
    }
}