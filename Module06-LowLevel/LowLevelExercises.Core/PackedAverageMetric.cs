using System.Threading;

namespace LowLevelExercises.Core
{
    /// <summary>
    /// This class assumes that the sum and the count of measurements won't breach <see cref="int.MaxValue"/> and packs them in a single field.
    /// </summary>
    public class PackedAverageMetric : IAverageMetric
    {
        // The upper 32 bits are used for the sum, the lower 32 for the count
        long packed;

        public void Report(int value)
        {
            var change = ((long)value << 32) | 1;
            Interlocked.Add(ref packed, change);
        }

        public double Average
        {
            get
            {
                var value = Volatile.Read(ref packed);

                return Calculate((int)(value >> 32), (int)value);
            }
        }

        static double Calculate(in int sum, in int count)
        {
            if (count == 0)
            {
                return double.NaN;
            }

            return (double)sum / count;
        }
    }
}