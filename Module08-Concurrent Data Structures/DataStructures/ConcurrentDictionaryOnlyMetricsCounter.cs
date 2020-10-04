using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DataStructures
{
    public class ConcurrentDictionaryOnlyMetricsCounter : IMetricsCounter
    {
        readonly ConcurrentDictionary<string, int> counters = new ConcurrentDictionary<string, int>();

        public IEnumerator<KeyValuePair<string, int>> GetEnumerator() => counters.GetEnumerator();

        public void Increment(string key)
        {
            counters.AddOrUpdate(key, 1, (_, existing) => existing + 1);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}