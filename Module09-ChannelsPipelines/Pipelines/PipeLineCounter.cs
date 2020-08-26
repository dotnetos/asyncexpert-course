using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pipelines
{
    public class PipeLineCounter
    {
        private const byte EOL = (byte)'\n';

        public async Task<int> CountLines(Uri uri)
        {
            using var client = new HttpClient();
            await using var stream = await client.GetStreamAsync(uri);

            // Calculate how many lines (end of line characters `\n`) are in the network stream
            // To practice, use a pattern where you have the Pipe, Writer and Reader tasks
            // Read about SequenceReader<T>, https://docs.microsoft.com/en-us/dotnet/api/system.buffers.sequencereader-1?view=netcore-3.1
            // This struct h has a method that can be very useful for this scenario :)

            // Good luck and have fun with pipelines!

            var pipe = new Pipe();
            var writer = FillPipe(stream, pipe.Writer);
            var reader = ReadLines(pipe.Reader);

            await Task.WhenAll(writer, reader);

            return reader.Result;
        }

        private static async Task<int> ReadLines(PipeReader reader)
        {
            int count = 0;
            while (true)
            {
                var result = await reader.ReadAsync();
                var buffer = result.Buffer;
                count += CountLinesInBuffer(buffer, count);

                reader.AdvanceTo(buffer.End);

                if (result.IsCompleted)
                {
                    break;
                }
            }

            return count;
        }

        private static int CountLinesInBuffer(ReadOnlySequence<byte> buffer, int count)
        {
            var sequenceReader = new SequenceReader<byte>(buffer);
            while (sequenceReader.TryAdvanceTo(EOL))
            {
                count++;
            }

            return count;
        }

        private static async Task FillPipe(Stream stream, PipeWriter writer)
        {
            while (true)
            {
                var buffer = writer.GetMemory();
                var bytes = await stream.ReadAsync(buffer);
                writer.Advance(bytes);

                if (bytes == 0)
                {
                    break;
                }

                var flush = await writer.FlushAsync();
                if (flush.IsCompleted || flush.IsCanceled)
                {
                    break;
                }
            }

            await writer.CompleteAsync();
        }
    }
}
