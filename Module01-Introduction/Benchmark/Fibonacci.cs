using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace Dotnetos.AsyncExpert.Homework.Module01.Benchmark
{
    [DisassemblyDiagnoser(exportCombinedDisassemblyReport: true)]
    public class FibonacciCalc
    {
        // HOMEWORK:
        // 1. Write implementations for RecursiveWithMemoization and Iterative solutions
        // 2. Add MemoryDiagnoser to the benchmark
        // 3. Run with release configuration and compare results
        // 4. Open disassembler report and compare machine code
        // 
        // You can use the discussion panel to compare your results with other students

        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(Data))]
        public ulong Recursive(ulong n)
        {
            if (n == 1 || n == 2) return 1;
            return Recursive(n - 2) + Recursive(n - 1);
        }

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public ulong RecursiveWithMemoization(ulong n)
        {
            if (n == 1 || n == 2) return 1;
            var results = new ulong[n];
            results[0] = 1;
            results[1] = 1;
            return RecursiveMemo(n, results);
        }

        private static ulong RecursiveMemo(ulong n, ulong[] results)
        {
            var current = n - 1;
            var previous = current - 1;
            var beforePrevious = previous - 1;

            if (results[beforePrevious] == 0)
            {
                results[beforePrevious] = RecursiveMemo(previous, results);
            }

            if (results[previous] == 0)
            {
                results[previous] = RecursiveMemo(current, results);
            }

            results[current] = results[beforePrevious] + results[previous];
            return results[n - 1];
        }

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public ulong Iterative(ulong n)
        {
            if (n == 1 || n == 2) return 1;

            ulong a = 1, b = 1;
            for (ulong i = 2; i < n; i++)
            {
                ulong temp = a + b;
                a = b;
                b = temp;
            }

            return b;
        }

        public IEnumerable<ulong> Data()
        {
            yield return 15;
            yield return 35;
        }
    }
}
