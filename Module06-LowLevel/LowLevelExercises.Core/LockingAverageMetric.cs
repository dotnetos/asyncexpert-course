using Microsoft.VisualBasic;

namespace LowLevelExercises.Core
{
    /// <summary>
    /// A simple class for reporting a specific value and obtaining an average.
    /// </summary>
    public class LockingAverageMetric : IAverageMetric
    {
        readonly object sync = new object();

        int sum = 0;
        int count = 0;

        public void Report(int value)
        {
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
