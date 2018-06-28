using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FclEx.Helpers;
using FclEx.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Xunit;

namespace FclEx.Test.Json
{
    public class KeyValuePairConverterTests
    {
        public static Type KvRawType { get; } = typeof(KeyValuePair<,>);
        public static IEnumerable<int> Source { get; } = Enumerable.Range(1, 100);
        public static IEnumerable<object[]> Cases { get; } = new object[]
        {
            Source.ToDictionary(m => m, m => -m),
            Source.ToDictionary(m => m, m => (-m).ToString()),
            Source.ToDictionary(m => m.ToString(), m => -m),
            Source.ToDictionary(m => m.ToString(), m => (-m).ToString())
        }.Select(m => new[] { m }).ToArray();

        private static void ReadTestGeneric<T, TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> raw)
            where T : IEnumerable<KeyValuePair<TKey, TValue>>
        {
            var json = raw.ToJson();
            var pairs = JsonConvert.DeserializeObject<T>(json, new KeyValuePairsConverter());

            var dic = raw.ToDictionary(m => m.Key, m => m.Value);

            var i = 0;
            foreach (var pair in pairs)
            {
                Assert.True(dic.TryGetValue(pair.Key, out var value));
                Assert.Equal(value, pair.Value);
                ++i;
            }
            Assert.Equal(dic.Count, i);
        }

        private static void ReadTest(IDictionary dic, Func<Type, Type> toColType)
        {
            var dicType = dic.GetType();
            var keyType = dicType.GenericTypeArguments[0];
            var valueType = dicType.GenericTypeArguments[1];
            var kvType = KvRawType.MakeGenericType(keyType, valueType);
            var colType = toColType(kvType);
            _method.MakeGenericMethod(colType, keyType, valueType)
                .Invoke(null, new object[] { dic });
        }

        private static readonly MethodInfo _method =
            typeof(KeyValuePairConverterTests).GetMethod(nameof(ReadTestGeneric));

        [Theory]
        [MemberData(nameof(Cases))]
        public void ReadTest_Array(IDictionary dic)
        {
            ReadTest(dic, t => t.MakeArrayType());
        }

        [Theory]
        [MemberData(nameof(Cases))]
        public void ReadTest_List(IDictionary dic)
        {
            ReadTest(dic, t => t.MakeArrayType());
        }

        [Theory]
        [MemberData(nameof(Cases))]
        public void ReadTest_Enum(IDictionary dic)
        {
            ReadTest(dic, t => t.MakeArrayType());
        }
    }
}
