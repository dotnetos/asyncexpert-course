using System.Collections.Generic;

namespace DataStructures
{
    public interface IMetricsCounter : IEnumerable<KeyValuePair<string, int>>
    {
        /// <summary>
        /// Increments the counter under the specific <paramref name="key"/>. If the key does not exist, it should be initialized with 1.
        /// </summary>
        /// <param name="key">The key</param>
        void Increment(string key);
    }
}