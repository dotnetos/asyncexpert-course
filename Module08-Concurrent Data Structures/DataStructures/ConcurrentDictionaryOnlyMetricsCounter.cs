using System.Collections;
using System.Collections.Generic;

namespace DataStructures
{
    public class ConcurrentDictionaryOnlyMetricsCounter : IMetricsCounter
    {
        // Implement this class using only ConcurrentDictionary.
        // Use methods that change the state atomically to ensure that everything is counted properly.
        // This task does not require using any Interlocked, or Volatile methods. The only required API is provided by the ConcurrentDictionary

        public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public void Increment(string key)
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}