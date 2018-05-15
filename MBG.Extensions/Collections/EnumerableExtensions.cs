using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;
using MBG.Extensions.Text;

namespace MBG.Extensions.Collections
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Determines whether this collection contains any of the specified values
        /// </summary>
        /// <typeparam name="T">The type of the values to compare</typeparam>
        /// <param name="t">This collection</param>
        /// <param name="items">The values to compare</param>
        /// <returns>true if the collection contains any of the specified values, otherwise false</returns>
        public static bool ContainsAny<T>(this IEnumerable<T> t, params T[] items)
        {
            foreach (T item in items)
            {
                if (t.Contains(item))
                { return true; }
            }
            return false;
        }

        /// <summary>
        /// Performs the specified action on each element of the System.Collections.Generic.IEnumerable&lt;T&gt;.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">This instance of System.Collections.Generic.IEnumerable&lt;T&gt;.</param>
        /// <param name="action">The System.Action&lt;T&gt; delegate to perform on each element of the System.Collections.Generic.IEnumerable&lt;T&gt;.</param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T item in enumerable)
            {
                action(item);
            }
        }

        /// <summary>
        /// Indicates whether the specified System.Collections.Generic.IEnumerable&lt;T&gt; is null or empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">This instance of System.Collections.Generic.IEnumerable&lt;T&gt;.</param>
        /// <returns>true if the System.Collections.Generic.IEnumerable&lt;T&gt; is null or empty; otherwise, false.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (typeof(T) == typeof(string))
            {
                return string.IsNullOrEmpty(enumerable as string); //TODO: Test
            }

            return enumerable == null || enumerable.Count() < 1;
        }

        /// <summary>
        /// <para>Returns all elements of this IEnumerable&lt;T&gt; in a single System.String.</para>
        /// <para>Elements are separated by a comma.</para>
        /// </summary>
        /// <param name="values">This instance of System.Collections.Generic.IEnumerable&lt;T&gt;.</param>
        /// <returns>System.String containing elements from specified IEnumerable&lt;T&gt;.</returns>
        public static string Join<T>(this IEnumerable<T> values)
        {
            return values.Join(",");
        }

        /// <summary>
        /// <para>Returns all elements of this IEnumerable&lt;T&gt; in a single System.String.</para>
        /// <para>Elements are separated by the specified separator.</para>
        /// </summary>
        /// <param name="values">This instance of System.Collections.Generic.IEnumerable&lt;T&gt;.</param>
        /// <param name="separator">The System.String to use to separate each element.</param>
        /// <returns>System.String containing elements from specified IEnumerable&lt;T&gt;.</returns>
        public static string Join<T>(this IEnumerable<T> values, string separator)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (separator == null)
            {
                separator = string.Empty;
            }
            using (IEnumerator<T> enumerator = values.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return string.Empty;
                }
                StringBuilder builder = new StringBuilder();
                if (enumerator.Current != null)
                {
                    builder.Append(enumerator.Current);
                }
                while (enumerator.MoveNext())
                {
                    builder.Append(separator);
                    if (enumerator.Current != null)
                    {
                        builder.Append(enumerator.Current);
                    }
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// <para>Returns all elements of this IEnumerable&lt;T&gt; in a single System.String.</para>
        /// <para>Elements are separated by a new line. Element properties are separated by </para>
        /// <para>a comma.</para>
        /// </summary>
        /// <typeparam name="T">The type of element contained in this IEnumerable&lt;T&gt;.</typeparam>
        /// <param name="enumerable">This instance of System.Collections.Generic.IEnumerable&lt;T&gt;.</param>
        /// <returns>System.String containing elements from specified IEnumerable&lt;T&gt;.</returns>
        public static string ToValueSeparatedList<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.ToValueSeparatedList(",");
        }

        /// <summary>
        /// <para>Returns all elements of this IEnumerable&lt;T&gt; in a single System.String.</para>
        /// <para>Elements are separated by a new line. Element properties are separated by </para>
        /// <para>the specified separator.</para>
        /// </summary>
        /// <typeparam name="T">The type of element contained in this IEnumerable&lt;T&gt;.</typeparam>
        /// <param name="enumerable">This instance of System.Collections.Generic.IEnumerable&lt;T&gt;.</param>
        /// <param name="separator">The System.String to use to separate each element.</param>
        /// <returns>System.String containing elements from specified IEnumerable&lt;T&gt;.</returns>
        public static string ToValueSeparatedList<T>(this IEnumerable<T> enumerable, string separator)
        {
            StringBuilder sb = new StringBuilder(200);

            IEnumerable<PropertyInfo> properties = typeof(T).GetProperties();

            #region If T Is String Or Has No Properties

            if (properties.IsNullOrEmpty() || typeof(T) == typeof(string))
            {
                sb.Append("Value");

                foreach (T item in enumerable)
                {
                    sb.Append(Environment.NewLine);
                    sb.Append(item.ToString());
                }

                return sb.ToString();
            }

            #endregion

            #region Else Normal Collection

            foreach (PropertyInfo property in properties)
            {
                sb.Append(property.Name, separator);
            }
            sb.Remove(sb.Length - separator.Length, separator.Length);

            foreach (T item in enumerable)
            {
                sb.Append(Environment.NewLine);

                foreach (PropertyInfo property in properties)
                {
                    if (property.PropertyType.IsEnum)
                    {
                        sb.Append(
                            Enum.ToObject(
                                property.PropertyType,
                                Convert.ToInt32(property.GetValue(item, null))).ToString(),
                            separator);
                    }
                    else { sb.Append(property.GetValue(item, null), separator); }
                }
                sb.Remove(sb.Length - separator.Length, separator.Length);
            }

            #endregion

            return sb.ToString();
        }

        public static Stack<T> ToStack<T>(this IEnumerable<T> enumerable)
        {
            Stack<T> stack = new Stack<T>();
            foreach (T item in enumerable.Reverse())
            {
                stack.Push(item);
            }
            return stack;
        }
        public static Queue<T> ToQueue<T>(this IEnumerable<T> enumerable)
        {
            Queue<T> queue = new Queue<T>();
            foreach (T item in enumerable)
            {
                queue.Enqueue(item);
            }
            return queue;
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> enumerable)
        {
            DataTable table = new DataTable();

            IEnumerable<PropertyInfo> properties = typeof(T).GetProperties();

            #region If T Is String Or Has No Properties

            if (properties.IsNullOrEmpty() || typeof(T) == typeof(string))
            {
                table.Columns.Add(new DataColumn("Value", typeof(string)));

                foreach (T item in enumerable)
                {
                    DataRow row = table.NewRow();

                    row["Value"] = item.ToString();

                    table.Rows.Add(row);
                }

                return table;
            }

            #endregion

            #region Else Normal Collection

            foreach (PropertyInfo property in properties)
            {
                table.Columns.Add(new DataColumn(property.Name, property.PropertyType));
            }

            foreach (T item in enumerable)
            {
                DataRow row = table.NewRow();

                foreach (PropertyInfo property in properties)
                {
                    row[property.Name] = property.GetValue(item, null);
                }

                table.Rows.Add(row);
            }

            #endregion

            return table;
        }

        // Thanks to the guys from http://signum.codeplex.com/
        public static string ToString<T>(this IEnumerable<T> collection, string separator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (T item in collection)
            {
                sb.Append(item.ToString());
                sb.Append(separator);
            }
            return sb.ToString(0, System.Math.Max(0, sb.Length - separator.Length));  // Remove at the end is faster
        }
        // Thanks to the guys from http://signum.codeplex.com/
        public static string ToString<T>(this IEnumerable<T> collection, Func<T, string> toString, string separator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (T item in collection)
            {
                sb.Append(toString(item));
                sb.Append(separator);
            }
            return sb.ToString(0, System.Math.Max(0, sb.Length - separator.Length));  // Remove at the end is faster
        }
    }
}