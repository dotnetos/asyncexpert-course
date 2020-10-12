using System.Threading;
using LowLevelExercises.Core;
using NUnit.Framework;

namespace LowLevelExercises.Tests
{
    public class AverageMetricTests
    {
        [Test]
        public void Returns_NaN_when_nothing()
        {
            Assert.AreEqual(double.NaN, new AverageMetric().Average);
        }

        [Test]
        public void Returns_Average()
        {
            var metric = new AverageMetric();
            metric.Report(1);
            metric.Report(3);
            Assert.AreEqual(2, metric.Average);
        }

        [Test]
        [Category("Long running")]
        [Category("Threading")]
        public void Eventually_returns_value_when_multiple_threads()
        {
            const int threadCount = 8;
            const int spinCount = 1000_000;

            var threads = new Thread[threadCount];
            var wait = new ManualResetEvent(false);

            var metric = new AverageMetric();

            for (var i = 0; i < threadCount; i++)
            {
                var threadNo = i;
                threads[i] = new Thread(() =>
                {
                    wait.WaitOne();

                    for (var j = 0; j < spinCount; j++)
                    {
                        metric.Report(threadNo % 2); // report 0 or 1
                    }
                });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            wait.Set();

            foreach (var thread in threads)
            {
                thread.Join();
            }

            Assert.AreEqual(0.5d, metric.Average);
        }
    }
}