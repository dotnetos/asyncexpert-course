using System;
using System.Threading;
using ThreadPoolExercises.Core;

namespace ThreadPoolExercises
{
    class Program
    {
        static void Main(string[] args)
        {
            // Here you can play around with those method, prototype and easily debug

            Console.WriteLine($"Main thread is {Thread.CurrentThread.ManagedThreadId}");

            ThreadingHelpers.ExecuteOnThread(() =>
            {
                var thread = Thread.CurrentThread;
                Console.WriteLine($"Hello from thread {thread.ManagedThreadId} from a pool: {thread.IsThreadPoolThread}");
            }, 3);

            ThreadingHelpers.ExecuteOnThreadPool(() =>
            {
                var thread = Thread.CurrentThread;
                Console.WriteLine(
                    $"Hello from thread {thread.ManagedThreadId} from a pool: {thread.IsThreadPoolThread}");
            }, 3);
        }
    }
}
