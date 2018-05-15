using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace MBG.Extensions.Core
{
    public static class ByteExtensions
    {
        /// <summary>
        /// Deserializes the Binary data contained in the specified System.Byte[].
        /// </summary>
        /// <typeparam name="T">The type of System.Object to be deserialized.</typeparam>
        /// <param name="data">This System.Byte[] instance.</param>
        /// <returns>The System.Object being deserialized.</returns>
        public static T BinaryDeserialize<T>(this byte[] data) where T : ISerializable
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                stream.Seek(0, SeekOrigin.Begin);
                T item = (T)binaryFormatter.Deserialize(stream);
                stream.Close();
                return item;
            }
        }

        /// <summary>
        /// Encrypts data with the System.Security.Cryptography.RSA algorithm.
        /// </summary>
        /// <param name="bytes">The data to be encrypted.</param>
        /// <param name="parameters">The parameters for System.Security.Cryptography.RSA.</param>
        /// <param name="doOAEPPadding">
        /// <para>true to perform direct System.Security.Cryptography.RSA encryption using</para>
        /// <para>OAEP padding (only available on a computer running Microsoft Windows XP or</para>
        /// <para>later); otherwise, false to use PKCS#1 v1.5 padding.</para>
        /// </param>
        /// <returns>The encrypted data.</returns>
        public static byte[] RSAEncrypt(this byte[] bytes, RSAParameters parameters, bool doOAEPPadding)
        {
            using (RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider())
            {
                rsaCryptoServiceProvider.ImportParameters(parameters);
                return rsaCryptoServiceProvider.Encrypt(bytes, doOAEPPadding);
            }
        }

        /// <summary>
        /// Decrypts data with the System.Security.Cryptography.RSA algorithm.
        /// </summary>
        /// <param name="bytes">The data to be decrypted.</param>
        /// <param name="parameters">The parameters for System.Security.Cryptography.RSA.</param>
        /// <param name="doOAEPPadding">
        /// <para>true to perform direct System.Security.Cryptography.RSA encryption using</para>
        /// <para>OAEP padding (only available on a computer running Microsoft Windows XP or</para>
        /// <para>later); otherwise, false to use PKCS#1 v1.5 padding.</para>
        /// </param>
        /// <returns>The decrypted data, which is the original plain text before encryption.</returns>
        public static byte[] RSADecrypt(this byte[] bytes, RSAParameters parameters, bool doOAEPPadding)
        {
            using (RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider())
            {
                rsaCryptoServiceProvider.ImportParameters(parameters);
                return rsaCryptoServiceProvider.Decrypt(bytes, doOAEPPadding);
            }
        }

        /// <summary>
        /// Encrypts the specified System.Byte[] using the TripleDES symmetric algorithm and returns the original System.String.
        /// </summary>
        /// <param name="data">The encrypted data to decrypt.</param>
        /// <param name="encoding">The System.Text.Encoding to use.</param>
        /// <param name="key">The secret key to use for the symmetric algorithm.</param>
        /// <param name="initializationVector">The initialization vector to use for the symmetric algorithm.</param>
        /// <returns>Decrypted System.Byte[] as a System.String.</returns>
        public static string TripleDESDecrypt(this byte[] data, Encoding encoding, byte[] key, byte[] initializationVector)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                using (CryptoStream cryptoStream = new CryptoStream(
                    memoryStream,
                    new TripleDESCryptoServiceProvider().CreateDecryptor(key, initializationVector),
                    CryptoStreamMode.Read))
                {
                    byte[] bytes = new byte[data.Length];
                    cryptoStream.Read(bytes, 0, bytes.Length);
                    return encoding.GetString(bytes);
                }
            }
        }
    }
}