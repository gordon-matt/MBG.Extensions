using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using MBG.Extensions.Collections;

namespace MBG.Extensions.Core
{
    public static partial class StringExtensions
    {
        public static string AddSingleQuotes(this string s)
        {
            return string.Concat("'", s, "'");
        }

        public static string AddDoubleQuotes(this string s)
        {
            return string.Concat("\"", s, "\"");
        }

        public static string Append(this string s, params string[] values)
        {
            string[] items = new string[values.Length + 1];
            items[0] = s;
            values.CopyTo(items, 1);
            return string.Concat(items);
        }

        public static string Append(this string s, params object[] values)
        {
            object[] items = new object[values.Length + 1];
            items[0] = s;
            values.CopyTo(items, 1);
            return string.Concat(items);
        }

        //Many thanks to Marc Clifton (http://www.codeproject.com/KB/string/stringhelpers.aspx)
        /// <summary>
        /// Returns the characters between and exclusive of the two search characters; [from] and [to].
        /// </summary>
        /// <param name="s">This System.String.</param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static string Between(this string s, char left, char right)
        {
            int indexFrom = s.IndexOf(left);
            if (indexFrom != -1)
            {
                ++indexFrom;
                int indexTo = s.IndexOf(right, indexFrom);
                if (indexTo != -1)
                {
                    return s.Substring(indexFrom, indexTo - indexFrom);
                }
            }
            return string.Empty;
        }

        public static int CharacterCount(this string s, char c)
        {
            return (from x in s.ToCharArray()
                    where x == c
                    select x).Count();
        }

        public static bool Contains(this string s, string value, StringComparison comparisonType)
        {
            return s.IndexOf(value, comparisonType) != -1;
        }

        /// <summary>
        /// <para>Returns a value indicating whether all of the specified System.String objects</para>
        /// <para>occur within this string.</para>
        /// </summary>
        /// <param name="s">The string</param>
        /// <param name="values">The strings to seek</param>
        /// <returns>true if all values are contained in this string; otherwise, false.</returns>
        public static bool ContainsAll(this string s, params string[] values)
        {
            foreach (string value in values)
            {
                if (!s.Contains(value))
                { return false; }
            }
            return true;
        }

        public static bool ContainsAll(this string s, params char[] values)
        {
            foreach (char value in values)
            {
                if (!s.Contains(value.ToString()))
                { return false; }
            }
            return true;
        }

        /// <summary>
        /// <para>Returns a value indicating whether any of the specified System.String objects</para>
        /// <para>occur within this string.</para>
        /// </summary>
        /// <param name="s">The string</param>
        /// <param name="values">The strings to seek</param>
        /// <returns>true if any value is contained in this string; otherwise, false.</returns>
        public static bool ContainsAny(this string s, params string[] values)
        {
            foreach (string value in values)
            {
                if (s.Contains(value))
                { return true; }
            }
            return false;
        }

        public static bool ContainsAny(this string s, params char[] values)
        {
            foreach (char value in values)
            {
                if (s.Contains(value.ToString()))
                { return true; }
            }
            return false;
        }

        public static bool ContainsAny(this string s, IEnumerable<string> values)
        {
            foreach (string value in values)
            {
                if (string.IsNullOrEmpty(value))
                { continue; }
                if (s.Contains(value))
                { return true; }
            }
            return false;
        }

        /// <summary>
        /// Converts a string that has been HTML-encoded for HTTP transmission into a decoded string.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>A decoded string</returns>
        public static string HtmlDecode(this string s)
        {
            return HttpUtility.HtmlDecode(s);
        }

        /// <summary>
        /// Converts a string to an HTML-encoded string.
        /// </summary>
        /// <param name="s">The string to encode.</param>
        /// <returns>An encoded string.</returns>
        public static string HtmlEncode(this string s)
        {
            return HttpUtility.HtmlEncode(s);
        }

        /// <summary>
        /// Removes all Html tags from the specified System.String
        /// </summary>
        /// <param name="s">The string to strip of html tags.</param>
        /// <returns>A System.String without html tags</returns>
        public static string HtmlStrip(this string s)
        {
            return RegexHtmlTag.Replace(s, string.Empty);
        }

        // NOTE: .NET 4 already has this (only use for .NET 3.5)
        public static bool IsNullOrWhiteSpace(this string value)
        {
            if (value != null)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (!char.IsWhiteSpace(value[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool IsPlural(this string word)
        {
            if (unpluralizables.Contains(word.ToLowerInvariant()))
            {
                return true;
            }

            foreach (KeyValuePair<string, string> kv in singularizations)
            {
                if (Regex.IsMatch(word, kv.Key))
                { return true; }
            }

            return false;
        }

        /// <summary>
        /// Gets specified number of characters from left of string
        /// </summary>
        /// <param name="s"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string Left(this string s, int count)
        {
            return s.Substring(0, count);
        }

        //Many thanks to Marc Clifton (http://www.codeproject.com/KB/string/stringhelpers.aspx)
        /// <summary>
        /// Returns all characters to the left of the first occurrence of [c] in this System.String.
        /// </summary>
        /// <param name="s">This System.String.</param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string LeftOf(this string s, char c)
        {
            int index = s.IndexOf(c);
            if (index != -1)
            {
                return s.Substring(0, index);
            }
            return s;
        }

        //Many thanks to Marc Clifton (http://www.codeproject.com/KB/string/stringhelpers.aspx)
        /// <summary>
        /// Returns all characters to the left of the [n]th occurrence of [c] in this System.String.
        /// </summary>
        /// <param name="s">This System.String.</param>
        /// <param name="c"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string LeftOf(this string s, char c, int n)
        {
            int index = -1;
            while (n > 0)
            {
                index = s.IndexOf(c, index + 1);
                if (index == -1)
                { break; }
                --n;
            }
            if (index != -1)
            {
                return s.Substring(0, index);
            }
            return s;
        }

        //Many thanks to Marc Clifton (http://www.codeproject.com/KB/string/stringhelpers.aspx)
        /// <summary>
        /// Returns all characters to the left of the last occurrence of [c] in this System.String.
        /// </summary>
        /// <param name="s">This System.String.</param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string LeftOfLastIndexOf(this string s, char c)
        {
            string ret = s;
            int index = s.LastIndexOf(c);
            if (index != -1)
            {
                ret = s.Substring(0, index);
            }
            return ret;
        }

        public static string Prepend(this string s, params string[] values)
        {
            string[] items = new string[values.Length + 1];
            values.CopyTo(items, 0);
            items[items.Length - 1] = s;
            return string.Concat(items);
        }

        public static string Prepend(this string s, params object[] values)
        {
            object[] items = new object[values.Length + 1];
            values.CopyTo(items, 0);
            items[items.Length - 1] = s;
            return string.Concat(items);
        }

        public static string Pluralize(this string singular)
        {
            if (unpluralizables.Contains(singular))
            { return singular; }

            string plural = string.Empty;
            foreach (KeyValuePair<string, string> kv in pluralizations)
            {
                if (Regex.IsMatch(singular, kv.Key))
                {
                    plural = Regex.Replace(singular, kv.Key, kv.Value);
                    break;
                }
            }

            return plural;
        }

        /// <summary>
        /// <para>Removes all spaces and tabs surrounding the specified substring contained</para>
        /// <para>within this System.String</para>
        /// </summary>
        /// <param name="s">The System.String to check</param>
        /// <param name="subString">The substring to remove whitespace from</param>
        /// <returns>System.String without whitespace around specified substring</returns>
        public static string RemoveSurroundingWhitespace(this string s, string subString)
        {
            string newString = s;

            while (newString.Contains(string.Concat(' ', subString)))
            { newString = newString.Replace(string.Concat(' ', subString), subString); }

            while (newString.Contains(string.Concat(subString, ' ')))
            { newString = newString.Replace(string.Concat(subString, ' '), subString); }

            while (newString.Contains(string.Concat('\t', subString)))
            { newString = newString.Replace(string.Concat('\t', subString), subString); }

            while (newString.Contains(string.Concat(subString, '\t')))
            { newString = newString.Replace(string.Concat(subString, '\t'), subString); }

            return newString;
        }

        public static string RegexDecode(this string s)
        {
            return Regex.Unescape(s);
        }

        public static string RegexEncode(this string s)
        {
            return Regex.Escape(s);
        }

        /// <summary>
        /// <para>Takes a System.String and returns a new System.String of the original</para>
        /// <para>repeated [n] number of times</para>
        /// </summary>
        /// <param name="s">The String</param>
        /// <param name="count">The number of times to repeat the String</param>
        /// <returns>A new System.String of the original repeated [n] number of times</returns>
        public static string Repeat(this string s, byte count)
        {
            if (count == 0)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder(s.Length * byte.MaxValue);
            for (int i = 0; i < count; i++)
            {
                sb.Append(s);
            }

            return sb.ToString();
        }

        // Thanks to the guys from http://signum.codeplex.com/
        /// <summary>
        /// <para>Replaces all occurrences of the specified System.Strings in this instance</para>
        /// </para>with specified System.Strings from the given System.Collections.Generic.IDictionary.</para>
        /// </summary>
        /// <param name="s">This System.String instance</param>
        /// <param name="replacements">The given IDictionary. Keys found in this System.String will be replaced by corresponding Values</param>
        /// <returns></returns>
        public static string Replace(this string str, IDictionary<string, string> replacements)
        {
            Regex regex = new Regex(replacements.Keys.ToString(a => "(" + Regex.Escape(a) + ")", "|"));
            return regex.Replace(str, m => replacements[m.Value]);
        }

        /// <summary>
        /// Gets specified number of characters from right of string
        /// </summary>
        /// <param name="s"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string Right(this string s, int count)
        {
            return s.Substring(s.Length - count, count);
        }

        //Many thanks to Marc Clifton (http://www.codeproject.com/KB/string/stringhelpers.aspx)
        /// <summary>
        /// Returns all characters to the right of the first occurrence of [c] in this System.String.
        /// </summary>
        /// <param name="s">This System.String.</param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string RightOf(this string s, char c)
        {
            int index = s.IndexOf(c);
            if (index != -1)
            {
                return s.Substring(index + 1);
            }
            return s;
        }

        //Many thanks to Marc Clifton (http://www.codeproject.com/KB/string/stringhelpers.aspx)
        /// <summary>
        /// Returns all characters to the right of the [n]th occurrence of [c] in this System.String.
        /// </summary>
        /// <param name="s">This System.String.</param>
        /// <param name="c"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string RightOf(this string s, char c, int n)
        {
            int index = -1;
            while (n > 0)
            {
                index = s.IndexOf(c, index + 1);
                if (index == -1)
                { break; }
                --n;
            }

            if (index != -1)
            {
                return s.Substring(index + 1);
            }
            return s;
        }

        //Many thanks to Marc Clifton (http://www.codeproject.com/KB/string/stringhelpers.aspx)
        /// <summary>
        /// Returns all characters to the right of the last occurrence of [c] in this System.String.
        /// </summary>
        /// <param name="s">This System.String.</param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string RightOfLastIndexOf(this string s, char c)
        {
            string ret = string.Empty;
            int index = s.LastIndexOf(c);
            if (index != -1)
            {
                ret = s.Substring(index + 1);
            }
            return ret;
        }

        public static string Singularize(this string word)
        {
            if (unpluralizables.Contains(word.ToLowerInvariant()))
            {
                return word;
            }

            foreach (KeyValuePair<string, string> kv in singularizations)
            {
                if (Regex.IsMatch(word, kv.Key))
                {
                    return Regex.Replace(word, kv.Key, kv.Value);
                }
            }

            return word;
        }

        // Thanks to the guys from http://signum.codeplex.com/
        /// <summary>
        /// <para>Give Pascal Text and will return separate words. For example:</para>
        /// <para>MyPascalText will become "My Pascal Text"</para>
        /// </summary>
        /// <param name="pascalText"></param>
        /// <returns></returns>
        public static string SpacePascal(this string pascalText)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < pascalText.Length; i++)
            {
                char a = pascalText[i];
                if (char.IsUpper(a) && i + 1 < pascalText.Length && !char.IsUpper(pascalText[i + 1]))
                {
                    if (sb.Length > 0)
                    { sb.Append(' '); }
                    sb.Append(a);
                }
                else { sb.Append(a); }

            }

            return sb.ToString();
        }

        /// <summary>
        /// <para>Determines whether the beginning of this string instance matches</para>
        /// <para>one of the specified strings.</para>
        /// </summary>
        /// <param name="s">The string</param>
        /// <param name="values">The strings to compare</param>
        /// <returns>true if any value matches the beginning of this string; otherwise, false.</returns>
        public static bool StartsWithAny(this string s, params string[] values)
        {
            foreach (string value in values)
            {
                if (s.StartsWith(value))
                { return true; }
            }
            return false;
        }

        /// <summary>
        /// Writes the instance of System.String to a new file or overwrites the existing file.
        /// </summary>
        /// <param name="s">The string to write to file.</param>
        /// <param name="filePath">The file to write the string to.</param>
        /// <returns>true if successful,; otherwise false.</returns>
        public static bool ToFile(this string s, string filePath)
        {
            FileStream fs = null;
            StreamWriter sw = null;

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                {
                    Directory.CreateDirectory(filePath);
                }

                fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                sw = new StreamWriter(fs);

                sw.Write(s);
                sw.Flush();
                return true;
            }
            catch (IOException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (sw != null)
                { sw.Close(); }

                if (fs != null)
                { fs.Close(); }
            }
        }

        public static IEnumerable<string> ToLines(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                return s.Split(new string[] { "\r\n", Environment.NewLine }, StringSplitOptions.None);
            }
            else { return new string[0]; }
        }

        public static string ToPascal(this string s)
        {
            return s.SpacePascal().ToTitleCase().Replace(" ", string.Empty);
        }

        /// <summary>
        /// Converts the specified string to Title Case using the Current Culture.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>The specified string converted to Title Case.</returns>
        public static string ToTitleCase(this string s)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s);
        }

        /// <summary>
        /// Converts the specified string to Title Case.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <param name="cultureInfo">The System.Globalization.CultureInfo to use for converting to Title Case.</param>
        /// <returns>The specified string converted to Title Case.</returns>
        public static string ToTitleCase(this string s, CultureInfo cultureInfo)
        {
            return cultureInfo.TextInfo.ToTitleCase(s);
        }

        /// <summary>
        /// Encrypts the specified System.String using the TripleDES symmetric algorithm and returns the data as a System.Byte[].
        /// </summary>
        /// <param name="s">The System.String to encrypt.</param>
        /// <param name="encoding">The System.Text.Encoding to use.</param>
        /// <param name="key">The secret key to use for the symmetric algorithm.</param>
        /// <param name="initializationVector">The initialization vector to use for the symmetric algorithm.</param>
        /// <returns>Encryped System.String as a System.Byte[].</returns>
        public static byte[] TripleDESEncrypt(this string s, Encoding encoding, byte[] key, byte[] initializationVector)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(
                     memoryStream,
                     new TripleDESCryptoServiceProvider().CreateEncryptor(key, initializationVector),
                     CryptoStreamMode.Write))
                {
                    byte[] bytes = encoding.GetBytes(s);

                    cryptoStream.Write(bytes, 0, bytes.Length);
                    cryptoStream.FlushFinalBlock();

                    return memoryStream.ToArray();
                }
            }
        }

        public static string UrlEncode(this string s)
        {
            return HttpUtility.UrlEncode(s);
        }

        public static string UrlDecode(this string s)
        {
            return HttpUtility.UrlDecode(s);
        }

        public static int WordCount(this string s)
        {
            return s.Split(' ').Count();
        }

        /// <summary>
        /// Deserializes the XML data contained by the specified System.String
        /// </summary>
        /// <typeparam name="T">The type of System.Object to be deserialized</typeparam>
        /// <param name="s">The System.String containing XML data</param>
        /// <returns>The System.Object being deserialized.</returns>
        public static T XmlDeserialize<T>(this string s)
        {
            object locker = new object();
            StringReader stringReader = new StringReader(s);
            XmlTextReader reader = new XmlTextReader(stringReader);
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                lock (locker)
                {
                    T item = (T)xmlSerializer.Deserialize(reader);
                    reader.Close();
                    return item;
                }
            }
            finally
            {
                if (reader != null)
                { reader.Close(); }
            }
        }
    }
}