using System.Collections.Generic;
using System.Linq;

namespace MBG.Extensions.Collections
{
    public static class DictionaryExtensions
    {
        // Thanks to the guys from http://signum.codeplex.com/
        public static V GetOrCreate<K, V>(this IDictionary<K, V> dictionary, K key, V value)
        {
            V result;
            if (!dictionary.TryGetValue(key, out result))
            {
                result = value;
                dictionary.Add(key, result);
            }
            return result;
        }
        // Thanks to the guys from http://signum.codeplex.com/
        public static Dictionary<V, K> Inverse<K, V>(this IDictionary<K, V> dictionary)
        {
            return dictionary.ToDictionary(k => k.Value, k => k.Key);
        }
        // Thanks to the guys from http://signum.codeplex.com/
        public static Dictionary<K, V> Union<K, V>(this IDictionary<K, V> dictionary, IDictionary<K, V> other)
        {
            Dictionary<K, V> result = new Dictionary<K, V>(dictionary);
            foreach (KeyValuePair<K, V> kv in other)
            {
                V value = result.GetOrCreate(kv.Key, kv.Value);
            }
            return result;
        }
    }
}