using System.Threading;

namespace LowLevelExercises.Core
{
    /// <summary>
    /// A simple class for reporting a specific value and obtaining an average.
    /// </summary>
    /// TODO: remove the locking and use <see cref="Interlocked"/> and <see cref="Volatile"/> to implement a lock-free implementation.
    public class AverageMetric
    {
        // TODO: this should not be needed, once you remove all the locks below
        readonly object sync = new object();

        int sum = 0;
        int count = 0;

        public void Report(int value)
        {
            // TODO: how to increment sum + count without locking?
            lock (sync)
            {
                sum += value;
                count += 1;
            }
        }

        public double Average
        {
            get
            {
                // TODO: how to access the values in a lock-free way?
                // let's assume that we can return value estimated on a bit stale data(in time average will be less and less diverged)
                lock (sync)
                {
                    return Calculate(count, sum);
                }
            }
        }

        static double Calculate(in int count, in int sum)
        {
            // DO NOT change the way calculation is done.

            if (count == 0)
            {
                return double.NaN;
            }

            return (double)sum / count;
        }
    }
}
