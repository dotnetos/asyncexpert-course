using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DataStructures.Tests
{
    [TestFixture(typeof(LockingMetricsCounter))]
    [TestFixture(typeof(ConcurrentDictionaryWithCounterMetricsCounter))]
    [TestFixture(typeof(ConcurrentDictionaryOnlyMetricsCounter))]
    public class MetricsCounterTest<TMetricCounter>
        where TMetricCounter : IMetricsCounter, new()
    {
        const int KeyCount = 16;
        const int ValueCount = 100000;
        const int ConcurrentWriters = 2;

        [Test]
        public async Task CountingTest()
        {
            var originalKeys = Enumerable.Range(0, KeyCount).Select(i => i.ToString()).ToArray();
            var keys = Enumerable.Repeat(originalKeys, ConcurrentWriters).SelectMany(m => m).ToArray();

            var starter = new TaskCompletionSource<object>();
            var counter = new TMetricCounter();

            // run two tasks per key
            var tasks = keys.Select(key => Task.Run(async () =>
            {
                await starter.Task;
                for (var i = 0; i < ValueCount; i++)
                {
                    counter.Increment(key);
                }
            })).ToArray();

            // start it
            starter.SetResult(starter);
            await Task.WhenAll(tasks).ConfigureAwait(false);

            // assert
            var toObserve = new HashSet<string>(keys);

            foreach (var (key, count) in counter)
            {
                Assert.AreEqual(ValueCount * ConcurrentWriters, count);
                Assert.IsTrue(toObserve.Remove(key), "A key that does not exist!");    
            }

            CollectionAssert.IsEmpty(toObserve);
        }
    }
}
