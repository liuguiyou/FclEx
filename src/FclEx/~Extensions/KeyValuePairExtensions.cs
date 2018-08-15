using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using FclEx.Utils;
using MoreLinq.Extensions;
using Newtonsoft.Json.Linq;

namespace FclEx
{
    public enum DupPolicy
    {
        OnlyLast = 0,
        OnlyFirst = 1,
        Throw = 2,
        Array
    }

    public static class KeyValuePairExtensions
    {
        public static void Deconstruct<TKey, TValue>(
            this KeyValuePair<TKey, TValue> kvp,
            out TKey key,
            out TValue value)
        {
            key = kvp.Key;
            value = kvp.Value;
        }

        public static KeyValuePair<string, string>[] ToPairs(this NameValueCollection col)
        {
            var q = from k in col.AllKeys
                    from v in col.GetValues(k).Touch()
                    select KvPair.For(k, v);
            return q.ToArray();
        }

        private static JToken ToJToken(HashSet<string> values, DupPolicy policy)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (values.Count == 0) throw new ArgumentException("the collection of values is empty", nameof(values));
            if (values.Count == 1) JToken.FromObject(values.First());
            switch (policy)
            {
                case DupPolicy.OnlyLast:
                    return JToken.FromObject(values.Last());
                case DupPolicy.OnlyFirst:
                    return JToken.FromObject(values.First());
                case DupPolicy.Throw:
                    throw new InvalidOperationException("the collection contains more than one value");
                case DupPolicy.Array:
                    return JArray.FromObject(values);
                default:
                    throw new ArgumentOutOfRangeException(nameof(policy), policy, null);
            }
        }

        public static JObject ToJObject(this IEnumerable<KeyValuePair<string, string>> pairs,
            DupPolicy policy = DupPolicy.OnlyLast)
        {
            var obj = new JObject();
            foreach (var pair in pairs.GroupBy(m => m.Key))
            {
                var values = pair.Select(m => m.Value).ToHashSet();
                if (values.Count > 0)
                    obj.Add(pair.Key, ToJToken(values, policy));
            }
            return obj;
        }

        public static JObject ToJObject(this NameValueCollection col, 
            DupPolicy policy = DupPolicy.OnlyLast)
        {
            var obj = new JObject();
            foreach (var k in col.AllKeys)
            {
                var values = col.GetValues(k).Touch().ToHashSet();
                if (values.Count > 0)
                    obj.Add(k, ToJToken(values, policy));
            }
            return obj;
        }
    }
}
