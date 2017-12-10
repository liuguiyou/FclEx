using System.Collections.Concurrent;

namespace FclEx
{
    public static class ConcurrentDictionaryExtensions
    {
        public static void Remove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dic, TKey key)
        {
            TValue value;
            dic.TryRemove(key, out value);
        }
    }
}
