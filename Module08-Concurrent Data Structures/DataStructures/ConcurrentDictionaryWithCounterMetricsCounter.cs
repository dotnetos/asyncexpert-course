using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DataStructures
{
    public class ConcurrentDictionaryWithCounterMetricsCounter : IMetricsCounter
    {
        readonly ConcurrentDictionary<string, AtomicCounter> counters = new ConcurrentDictionary<string, AtomicCounter>();
        
        public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
        {
            return counters.Select(kvp => new KeyValuePair<string, int>(kvp.Key, kvp.Value.Count)).GetEnumerator();
        }

        public void Increment(string key)
        {
            counters.GetOrAdd(key, _ => new AtomicCounter()).Increment();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public class AtomicCounter
        {
            int value;

            public void Increment() => Interlocked.Increment(ref value);

            public int Count => Volatile.Read(ref value);
        }
    }
}