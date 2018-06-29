using System;
using BenchmarkDotNet.Running;

namespace FclEx.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<GetDefaultTest>();
            Console.Read();
        }
    }
}
