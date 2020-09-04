using System;
using System.Threading;

namespace Synchronization.Core
{
    /*
     * Implement very simple wrapper around Mutex or Semaphore (remember both implement WaitHandle) to
     * provide a exclusive region created by `using` clause.
     *
     * Created region may be system-wide or not, depending on the constructor parameter.
     */
    public class NamedExclusiveScope : IDisposable
    {
        private WaitHandle _handle;

        public NamedExclusiveScope(string name, bool isSystemWide)
        {
            if (isSystemWide)
            {
                _handle = new Mutex(initiallyOwned: false, name, out bool createdNew);
                if (!createdNew)
                    throw new InvalidOperationException($"Unable to get a global lock {name}.");
            }
            else
            {
                _handle = new Semaphore(initialCount: 1, maximumCount: 1);
            }
        }

        public void Dispose()
        {
            _handle.Dispose();
        }
    }
}
