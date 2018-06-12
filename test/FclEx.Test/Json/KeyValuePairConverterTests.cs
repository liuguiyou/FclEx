using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FclEx.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Xunit;

namespace FclEx.Test.Json
{
    public class KeyValuePairConverterTests
    {
        //[Fact]
        //public void Read_Test()
        //{
        //    var dic = Enumerable.Range(1, 100).ToDictionary(m => m, m => -m);
        //    var json = dic.ToJson();

        //    var pairs = JsonConvert.DeserializeObject<KeyValuePair<int, int>[]>(json, new KeyValuePairsConverter());

        //    Assert.Equal(dic.Count, pairs.Length);
        //    foreach (var pair in pairs)
        //    {
        //        Assert.True(dic.TryGetValue(pair.Key, out var value));
        //        Assert.Equal(value, pair.Value);
        //    }
        //}
    }
}
