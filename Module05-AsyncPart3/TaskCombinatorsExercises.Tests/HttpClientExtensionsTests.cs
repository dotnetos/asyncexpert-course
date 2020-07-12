using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using TaskCombinatorsExercises.Core;
using Xunit;

namespace TaskCombinatorsExercises.Tests
{
    public class HttpClientExtensionsTests
    {
        [Fact]
        public async Task GivenSingleCall_ThenSucceeds()
        {
            var mockHttp = new MockHttpMessageHandler();
            var mockedRequest1 = GivenDelayUrl(mockHttp, 700);

            var result = await mockHttp.ToHttpClient().ConcurrentDownloadAsync(new[]
            {
                "https://local/delay/700"
            }, 10_000, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("700", result);
            Assert.Equal(1, mockHttp.GetMatchCount(mockedRequest1));
        }

        [Fact]
        public async Task GivenMultipleCalls_ThenSucceeds()
        {
            var mockHttp = new MockHttpMessageHandler();
            var mockedRequest1 = GivenDelayUrl(mockHttp, 100);
            var mockedRequest2 = GivenDelayUrl(mockHttp, 300);
            var mockedRequest3 = GivenDelayUrl(mockHttp, 700);

            var mockedClient = mockHttp.ToHttpClient();
            var result = await mockedClient.ConcurrentDownloadAsync(new[]
            {
                "https://local/delay/700",
                "https://local/delay/300",
                "https://local/delay/100"
            }, 10_000, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("100", result);
            Assert.Equal(1, mockHttp.GetMatchCount(mockedRequest1));
            Assert.Equal(1, mockHttp.GetMatchCount(mockedRequest2));
            Assert.Equal(1, mockHttp.GetMatchCount(mockedRequest3));
        }

        [Fact]
        public async Task GivenSingleCall_WhenTimeoutShorter_ThenThrows()
        {
            var mockHttp = new MockHttpMessageHandler();
            var mockedRequest1 = GivenDelayUrl(mockHttp, 1000);

            var exception = await Record.ExceptionAsync(async () =>
                await mockHttp.ToHttpClient().ConcurrentDownloadAsync(new[]
                {
                    "https://local/delay/1000"
                }, 100, CancellationToken.None));

            Assert.Equal(1, mockHttp.GetMatchCount(mockedRequest1));
            Assert.NotNull(exception);
            Assert.IsType<TaskCanceledException>(exception);
        }

        [Fact]
        public async Task GivenSingleCall_WhenCancelled_ThenThrows()
        {
            var mockHttp = new MockHttpMessageHandler();
            var mockedRequest1 = GivenDelayUrl(mockHttp, 1000);

            var cts = new CancellationTokenSource();
            cts.CancelAfter(100);

            var exception = await Record.ExceptionAsync(async () =>
                await mockHttp.ToHttpClient().ConcurrentDownloadAsync(new[]
                {
                    "https://local/delay/1000"
                }, 10_000, cts.Token));

            Assert.Equal(1, mockHttp.GetMatchCount(mockedRequest1));
            Assert.NotNull(exception);
            Assert.IsType<TaskCanceledException>(exception);
        }

        private IMockedRequest GivenDelayUrl(MockHttpMessageHandler mockHttp, int delay)
        {
            var mockedRequest = mockHttp.When($"/delay/{delay}");
            mockedRequest
                .Respond(async () =>
                {
                    await Task.Delay(delay);
                    return new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(delay.ToString())
                    };
                });
            return mockedRequest;
        }
    }
}
