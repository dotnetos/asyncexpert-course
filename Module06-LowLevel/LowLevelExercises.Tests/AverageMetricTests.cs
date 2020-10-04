using System.Threading.Tasks;
using LowLevelExercises.Core;
using NUnit.Framework;

namespace LowLevelExercises.Tests
{
    [TestFixture(typeof(LockingAverageMetric))]
    [TestFixture(typeof(EventuallyConsistentAverageMetric))]
    [TestFixture(typeof(PackedAverageMetric))]
    public class AverageMetricTests<TAverageMetric>
        where TAverageMetric : IAverageMetric, new()
    {
        [Test]
        public void Returns_NaN_when_nothing()
        {
            Assert.AreEqual(double.NaN, new TAverageMetric().Average);
        }

        [Test]
        public void Returns_Average()
        {
            var metric = new TAverageMetric();
            metric.Report(1);
            metric.Report(3);
            Assert.AreEqual(2, metric.Average);
        }

        [Test]
        public async Task CountingTest()
        {
            const int count = 1000000;
            
            var starter = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            var metric = new TAverageMetric();

            var reporting1 = Task.Run(async () =>
            {
                await starter.Task;
                for (var i = 0; i < count; i++)
                {
                    metric.Report(1);
                }
            });

            var reporting3 = Task.Run(async () =>
            {
                await starter.Task;
                for (var i = 0; i < count; i++)
                {
                    metric.Report(3);
                }
            });

            // start it
            starter.SetResult(starter);
            await Task.WhenAll(reporting1, reporting3).ConfigureAwait(false);

            Assert.AreEqual(2, metric.Average);
        }
    }
}