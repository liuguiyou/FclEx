using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace FclEx.Benchmark
{
    public class CreateObject
    {
        private static readonly Type _type = typeof(List<>);

        [Benchmark]
        public void CreateInstance()
        {
            Activator.CreateInstance(_type.MakeGenericType(typeof(int)), 4);
        }

        [Benchmark]
        public void Ctor()
        {
            
        }
    }
}
