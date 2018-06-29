using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;
using FclEx;

namespace FclEx.Benchmark
{
    [MemoryDiagnoser]
    public class GetDefaultTest
    {
        public static IEnumerable<object[]> Cases => new[]
        {
            typeof(int),
            typeof(string),
            typeof(DateTime),
            typeof(List<int>),
        }.Select(m => new object[] { m }).ToArray();

        [Benchmark]
        [ArgumentsSource(nameof(Cases))]
        public void GetDefault(Type type)
        {
            type.GetDefault();
        }

        [Benchmark]
        [ArgumentsSource(nameof(Cases))]
        public void GetDefaultByExp(Type type)
        {
            type.GetDefaultByExp();
        }
    }
}
