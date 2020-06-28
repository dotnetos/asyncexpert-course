using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AsyncAwaitExercises.Core;
using RichardSzalay.MockHttp;
using Xunit;

namespace AsyncAwaitExercises.Tests
{
    /*
     * IMPORTANT NOTE:
     * Those tests will take seconds because of the retry logic inside GetStringWithRetries
     * method. This is a special case for the purpose of this exercise but in general should
     * be considered an anti-pattern, or at least very special case of "integration" tests.
     *
     * To write proper unit tests, GetStringWithRetries (or an object/service with such a method)
     * should take a dependency to "delay provider" that could be mocked and verified for calls
     * during tests. Production code would use Task.Delay-based version.
     */
    public class GetStringWithRetriesTests
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public async Task LessThanTwoRetriesIsInvalidTest(int maxTries)
        {
            var mockHttp = new MockHttpMessageHandler();
            var client = mockHttp.ToHttpClient();

            var exception = await Record.ExceptionAsync(async () =>
                await AsyncHelpers.GetStringWithRetries(client,
                    "https://local/test",
                    maxTries));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        }

        [Fact]
        public async Task ImmediateCancellationTest()
        {
            var mockHttp = new MockHttpMessageHandler();
            var client = mockHttp.ToHttpClient();

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.Cancel();

            var exception = await Record.ExceptionAsync(async () => 
             await AsyncHelpers.GetStringWithRetries(client, 
                "https://local/test",
                token: cts.Token));

            Assert.NotNull(exception);
            Assert.IsType<TaskCanceledException>(exception);
        }

        [Fact]
        public async Task GivenSuccess_NoRetries_Test()
        {
            var mockHttp = new MockHttpMessageHandler();
            // `Expect` set up mock for single call only, so if GetStringWithRetries will
            // by mistake retry, it will get 404 for the further calls.
            mockHttp
                .Expect("/test")
                .Respond(HttpStatusCode.OK, "application/json", "test_data");

            var client = mockHttp.ToHttpClient();

            var result = await AsyncHelpers.GetStringWithRetries(client,
                "https://local/test", maxTries: 3);

            Assert.NotNull(result);
            Assert.Equal("test_data", result);
            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GivenFailAndSuccess_SingleRetry_Test()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .Expect("/test")
                .Respond(HttpStatusCode.InternalServerError);
            mockHttp
                .Expect("/test")
                .Respond(HttpStatusCode.OK, "application/json", "test_data");

            var client = mockHttp.ToHttpClient();

            var result = await AsyncHelpers.GetStringWithRetries(client,
                "https://local/test", maxTries: 3);

            Assert.NotNull(result);
            Assert.Equal("test_data", result);
            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GivenTwoFailsAndSuccess_TwoRetries_Test()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .Expect("/test")
                .Respond(HttpStatusCode.InternalServerError);
            mockHttp
                .Expect("/test")
                .Respond(HttpStatusCode.InternalServerError);
            mockHttp
                .Expect("/test")
                .Respond(HttpStatusCode.OK, "application/json", "test_data");

            var client = mockHttp.ToHttpClient();

            var result = await AsyncHelpers.GetStringWithRetries(client,
                "https://local/test", maxTries: 3);

            Assert.NotNull(result);
            Assert.Equal("test_data", result);
            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GivenThreeFails_TwoRetries_AndThrow_Test()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .Expect("/test")
                .Respond(HttpStatusCode.InternalServerError);
            mockHttp
                .Expect("/test")
                .Respond(HttpStatusCode.InternalServerError);
            mockHttp
                .Expect("/test")
                .Respond(HttpStatusCode.InternalServerError);

            var client = mockHttp.ToHttpClient();

            var exception = await Record.ExceptionAsync(async () =>
                await AsyncHelpers.GetStringWithRetries(client,
                    "https://local/test", maxTries: 3));

            Assert.NotNull(exception);
            Assert.IsType<HttpRequestException>(exception);
            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task AfterFirstTryCancellationTest()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .Expect("/test")
                .Respond(HttpStatusCode.InternalServerError);
            var client = mockHttp.ToHttpClient();

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(500);

            var exception = await Record.ExceptionAsync(async () =>
                await AsyncHelpers.GetStringWithRetries(client,
                    "https://local/test",
                    token: cts.Token));

            Assert.NotNull(exception);
            Assert.IsType<TaskCanceledException>(exception);
            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task AfterSecondTryCancellationTest()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .Expect("/test")
                .Respond(HttpStatusCode.InternalServerError);
            mockHttp
                .Expect("/test")
                .Respond(HttpStatusCode.InternalServerError);
            var client = mockHttp.ToHttpClient();

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(1500);

            var exception = await Record.ExceptionAsync(async () =>
                await AsyncHelpers.GetStringWithRetries(client,
                    "https://local/test",
                    token: cts.Token));

            Assert.NotNull(exception);
            Assert.IsType<TaskCanceledException>(exception);
            mockHttp.VerifyNoOutstandingExpectation();
        }
    }
}
