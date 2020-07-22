using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace SmallConcurrentQueue
{
    /// <summary>
    /// A really small concurrent queue, allowing ONE writer and ONE reader.
    /// This is just an example for understanding how <see cref="ConcurrentQueue{T}"/> works. I encourage strongly to use the default Multi Producer Multi Consumer version provided by .NET itself.
    /// </summary>
    public class SmallConcurrentQueue<T>
    {
        public const int Size = 16;
        const int SlotsMask = Size - 1;

        readonly Slot[] slots = new Slot[Size];
        PaddedHeadAndTail headAndTail;

        public SmallConcurrentQueue()
        {
            // Initialize the sequence number for each slot.
            //
            // A writer at position N can enqueue when the sequence number is N.
            // A reader at position N can dequeue when the sequence number is N + 1.
            //
            // When a writer finishes writing at position N, it sets the sequence number to N + 1, satisfying the reader condition.
            // When a reader finishes reading at position N, it sets the sequence number to N + Size, satisfying the writer condition.
            // 
            // To get slot number N, it uses the modulo operation.
            for (var i = 0; i < Size; i++)
            {
                slots[i].SequenceNumber = i;
            }
        }

        public bool TryEnqueue(T item)
        {
            var currentTail = headAndTail.Tail;
            var i = currentTail & SlotsMask;

            // Read the sequence number for the tail position.
            var sequenceNumber = Volatile.Read(ref slots[i].SequenceNumber);

            // The slot is empty and ready for us to enqueue into it if its sequence
            // number matches the slot.
            int diff = sequenceNumber - currentTail;
            if (diff == 0)
            {
                headAndTail.Tail += 1;

                slots[i].Item = item;

                // half barrier here! Writing the Item will never be reordered AFTER writing SequenceNumber. 
                // SequenceNumber is our status here!
                Volatile.Write(ref slots[i].SequenceNumber, currentTail + 1);
                return true;
            }

            // The slot is still occupied, by previous value
            return false;
        }

        /// <summary>Tries to dequeue an element from the queue.</summary>
        public bool TryDequeue(out T item)
        {
            // Get the head at which to try to dequeue.
            var currentHead = headAndTail.Head;
            var i = currentHead & SlotsMask;

            // Read the sequence number for the head position.
            int sequenceNumber = Volatile.Read(ref slots[i].SequenceNumber);

            // We can dequeue from this slot if it's been filled by an enqueuer, which
            // would have left the sequence number at pos+1.
            int diff = sequenceNumber - (currentHead + 1);
            if (diff == 0)
            {
                headAndTail.Head += 1;

                // Get the item
                item = slots[i].Item;

                // clear the item
                slots[i].Item = default;
                Volatile.Write(ref slots[i].SequenceNumber, currentHead + Size);

                return true;
            }

            item = default;
            return false;
        }

        [DebuggerDisplay("Item = {Item}, SequenceNumber = {SequenceNumber}")]
        [StructLayout(LayoutKind.Auto)]
        struct Slot
        {
            public T Item;
            public int SequenceNumber;
        }
    }

    [DebuggerDisplay("Head = {Head}, Tail = {Tail}")]
    [StructLayout(LayoutKind.Explicit, Size = 3 * AssumedMaxCacheLineSize)] // padding before/between/after fields
    struct PaddedHeadAndTail
    {
        const int AssumedMaxCacheLineSize = 128;

        /// <summary>
        /// Keeps the head
        /// </summary>
        [FieldOffset(1 * AssumedMaxCacheLineSize)] public int Head;
        [FieldOffset(2 * AssumedMaxCacheLineSize)] public int Tail;
    }
}