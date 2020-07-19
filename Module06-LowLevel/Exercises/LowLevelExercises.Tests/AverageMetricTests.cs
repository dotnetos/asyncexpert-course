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
    }
}