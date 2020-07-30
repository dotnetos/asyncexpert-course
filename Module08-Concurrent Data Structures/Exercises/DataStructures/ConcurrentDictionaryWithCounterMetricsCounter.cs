using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace DataStructures
{
    public class ConcurrentDictionaryWithCounterMetricsCounter : IMetricsCounter
    {
        // Implement this class using ConcurrentDictionary and the provided AtomicCounter class.
        // AtomicCounter should be created only once per key, then its Increment method should be used.
        
        public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public void Increment(string key)
        {
            throw new System.NotImplementedException();
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