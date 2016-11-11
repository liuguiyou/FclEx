﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FxUtility.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, TValue defaultValue = default(TValue))
        {
            TValue value;
            return dic.TryGetValue(key, out value) ? value : defaultValue;
        }

        public static void AddOrDo<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, TValue value, Action<TKey> action = null)
        {
            if (dic.ContainsKey(key)) action?.Invoke(key);
            else dic.Add(key, value);
        }

        public static void Add<TCol, TKey, TValue>(this IDictionary<TKey, TCol> dic, TKey key, TValue value) where TCol : ICollection<TValue>, new()
        {
            if (dic.ContainsKey(key) && dic[key] != null)
            {
                dic[key].Add(value);
            }
            else
            {
                dic[key] = new TCol { value };
            }
        }

        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dic, IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            foreach (var pair in pairs)
            {
                if(!dic.ContainsKey(pair.Key))
                {
                    dic.Add(pair);
                }
            }
        }

        public static void AddOrUpdateRange<TKey, TValue>(this IDictionary<TKey, TValue> dic, IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            foreach (var pair in pairs)
            {
                dic[pair.Key] = pair.Value;
            }
        }

        public static string ToQueryString(this IDictionary<string, string> dic)
        {
            return dic.IsNullOrEmpty() ? string.Empty :
                string.Join("&", dic.Select(item => $"{item.Key.UrlEncode()}={item.Value.UrlEncode()}"));
        }

        public static bool GetAndDo<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, Action<TValue> action)
        {
            var item = dic.GetOrDefault(key);
            if (item != null)
            {
                action(item);
                return true;
            }
            else return false;
        }
    }
}
