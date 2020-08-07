using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Pipelines.Tests
{
    public class PipeLineCounterTests
    {
        [Fact]
        public async Task CountLines()
        {
            var url = new Uri("https://www.w3.org/TR/PNG/iso_8859-1.txt");
            var expected = (await GetBody(url)).Count(x => x == '\n');

            var pipelinesCounter = new PipeLineCounter();
            var result = await pipelinesCounter.CountLines(url);

            Assert.Equal(expected, result);
        }

        private static async Task<string> GetBody(Uri uri)
        {
            using var client = new HttpClient();
            return await client.GetStringAsync(uri);
        }
    }
}