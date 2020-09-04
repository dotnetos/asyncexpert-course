using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitExercises.Core
{
    public class AsyncHelpers
    {
        public static async Task<string> GetStringWithRetries(HttpClient client, string url, int maxTries = 3, CancellationToken token = default)
        {
            // Create a method that will try to get a response from a given `url`, retrying `maxTries` number of times.
            // It should wait one second before the second try, and double the wait time before every successive retry
            // (so pauses before retries will be 1, 2, 4, 8, ... seconds).
            // * `maxTries` must be at least 2
            // * we retry if:
            //    * we get non-successful status code (outside of 200-299 range), or
            //    * HTTP call thrown an exception (like network connectivity or DNS issue)
            // * token should be able to cancel both HTTP call and the retry delay
            // * if all retries fails, the method should throw the exception of the last try
            // HINTS:
            // * `HttpClient.GetAsync` does not accept cancellation token (use `GetAsync` instead)
            // * you may use `EnsureSuccessStatusCode()` method
            static async Task<string> CancellableGetAsync(HttpClient client, string url, CancellationToken token)
            {
                var message = await client.GetAsync(url, HttpCompletionOption.ResponseContentRead, token);
                message.EnsureSuccessStatusCode();
                return await message.Content.ReadAsStringAsync();
            }
            if (maxTries < 2)
                throw new ArgumentException(nameof(maxTries));

            int delayMs = 1000;
            for (int i = 0; i < maxTries - 1; i++)
            {
                try
                {
                    return await CancellableGetAsync(client, url, token);
                }
                catch
                {
                    await Task.Delay(delayMs, token);
                    delayMs *= 2;
                }
            }
            // Last chance call, it will probably propagate (throw) the same exception
            await Task.Delay(delayMs, token);
            return await CancellableGetAsync(client, url, token);
        }

    }
}
