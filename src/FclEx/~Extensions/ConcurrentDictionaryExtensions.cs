using System.Collections.Concurrent;

namespace FclEx
{
    public static class ConcurrentDictionaryExtensions
    {
        public static void Remove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dic, TKey key)
        {
            dic.TryRemove(key, out _);
        }
    }
}
