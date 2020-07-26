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
        public NamedExclusiveScope(string name, bool isSystemWide)
        {
        }
    }
}
