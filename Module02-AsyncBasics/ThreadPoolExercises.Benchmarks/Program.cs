using System;
using BenchmarkDotNet.Running;

namespace ThreadPoolExercises.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<ThreadingHelpersBenchmarks>();
        }
    }
}
