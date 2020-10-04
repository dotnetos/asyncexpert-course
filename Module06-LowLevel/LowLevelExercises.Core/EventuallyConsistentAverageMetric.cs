using System.Threading;

namespace LowLevelExercises.Core
{
    public class EventuallyConsistentAverageMetric : IAverageMetric
    {
        int sum = 0;
        int count = 0;

        public void Report(int value)
        {
            Interlocked.Add(ref sum, value);
            Interlocked.Increment(ref count);
        }

        public double Average
        {
            get
            {
                var c = Volatile.Read(ref count);

                // The sum is read without any barriers because volatile above ensures, that the sum will be read with a value that is not older than the count.
                // In other words, the READ of the sum will not be moved above the previous line.
                // This may return value that is newer than the count, but for average this should be ok-ish.
                var s = sum; 

                return Calculate(c, s);
            }
        }

        static double Calculate(in int count, in int sum)
        {
            if (count == 0)
            {
                return double.NaN;
            }

            return (double)sum / count;
        }
    }
}