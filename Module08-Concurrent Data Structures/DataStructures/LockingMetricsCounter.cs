using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataStructures
{
    public class LockingMetricsCounter : IMetricsCounter
    {
        readonly Dictionary<string, int> counters = new Dictionary<string, int>();

        /// <summary>
        /// Increments the counter under the specific <paramref name="key"/>. If the key does not exist, it should be initialized with 1.
        /// </summary>
        /// <param name="key">The key</param>
        public void Increment(string key)
        {
            lock (counters)
            {
                if (counters.TryGetValue(key, out var counter))
                {
                    counters[key] = counter + 1;
                }
                else
                {
                    counters[key] = 1;
                }
            }
        }

        public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
        {
            lock (counters)
            {
                return counters.ToList().GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
