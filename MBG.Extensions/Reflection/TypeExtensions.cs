using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MBG.Extensions.Collections;
using System.Collections;

namespace MBG.Extensions.Reflection
{
    public static class TypeExtensions
    {
        public static object GetDefaultValue(this Type type)
        {
            if (type == typeof(Boolean)) return default(Boolean);
            if (type == typeof(Byte)) return default(Byte);
            if (type == typeof(Char)) return default(Char);
            if (type == typeof(Int16)) return default(Int16);
            if (type == typeof(Int32)) return default(Int32);
            if (type == typeof(Int64)) return default(Int64);
            if (type == typeof(Decimal)) return default(Decimal);
            if (type == typeof(Double)) return default(Double);
            if (type == typeof(DateTime)) return default(DateTime);
            if (type == typeof(Guid)) return default(Guid);
            if (type == typeof(Single)) return default(Single);
            if (type == typeof(String)) return default(String);
            if (type == typeof(SByte)) return default(SByte);
            if (type == typeof(TimeSpan)) return default(TimeSpan);
            if (type == typeof(UInt16)) return default(UInt16);
            if (type == typeof(UInt32)) return default(UInt32);
            if (type == typeof(UInt64)) return default(UInt64);
            if (type == typeof(Uri)) return default(Uri);
            if (type.IsEnum) return 0;
            return null;
        }

        public static IEnumerable<MethodInfo> GetExtensionMethods(this Type type, Assembly extensionsAssembly)
        {
            var query = from t in extensionsAssembly.GetTypes()
                        where !t.IsGenericType && !t.IsNested
                        from m in t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        where m.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false)
                        where m.GetParameters()[0].ParameterType == type
                        select m;

            return query;
        }

        public static MethodInfo GetExtensionMethod(this Type type, Assembly extensionsAssembly, string name)
        {
            return type.GetExtensionMethods(extensionsAssembly).Single(m => m.Name == name);
        }
        public static MethodInfo GetExtensionMethod(this Type type, Assembly extensionsAssembly, string name, Type[] types)
        {
            IEnumerable<MethodInfo> methods = (from m in type.GetExtensionMethods(extensionsAssembly)
                                               where m.Name == name
                                               && m.GetParameters().Count() == types.Length + 1 // + 1 because extension method parameter (this)
                                               select m);

            if (methods.IsNullOrEmpty())
            {
                return default(MethodInfo);
            }

            if (methods.Count() == 1)
            {
                return methods.First();
            }

            foreach (MethodInfo methodInfo in methods)
            {
                ParameterInfo[] parameters = methodInfo.GetParameters();

                bool found = true;
                for (byte b = 0; b < types.Length; b++)
                {
                    found = true;
                    if (parameters[b].GetType() != types[b])
                    {
                        found = false;
                        continue;
                    }
                }

                if (found)
                {
                    return methodInfo;
                }
            }

            return default(MethodInfo);
        }

        public static bool IsCollection(this Type type)
        {
            // string implements IEnumerable, but for our purposes we don't consider it a collection.
            if (type == typeof(string)) return false;

            var interfaces = from inf in type.GetInterfaces()
                             where inf == typeof(IEnumerable) ||
                                 (inf.IsGenericType && inf.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                             select inf;
            return interfaces.Count() != 0;
        }
        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType &&
                   type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
