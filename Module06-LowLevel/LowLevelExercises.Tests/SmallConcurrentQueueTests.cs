using System.Threading;
using NUnit.Framework;
using SmallConcurrentQueue;

namespace LowLevelExercises.Tests
{
    public class SmallConcurrentQueueTests
    {
        [Test]
        public void Load_Test()
        {
            const int count = 1000_000;

            var queue = new SmallConcurrentQueue<int>();

            var publisher = new Thread(() =>
            {
                for (var i = 0; i < count; i++)
                {
                    if (queue.TryEnqueue(i) == false)
                    {
                        // queue is full: dummy fast retry
                        i--;
                    }
                }
            });

            var consumer = new Thread(() =>
            {
                for (var i = 0; i < count; i++)
                {
                    if (queue.TryDequeue(out var value))
                    {
                        Assert.AreEqual(i, value);
                    }
                    else
                    {
                        // queue is empty: dummy fast retry
                        i--;
                    }
                }
            });

            publisher.Start();
            consumer.Start();

            publisher.Join();
            consumer.Join();
        }
    }
}