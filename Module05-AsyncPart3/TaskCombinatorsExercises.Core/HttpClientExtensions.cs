using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TaskCombinatorsExercises.Core
{
    public static class HttpClientExtensions
    {
        /*
         Write cancellable async method with timeout handling, that concurrently tries to get data from
         provided urls (first wins and its response is returned, rest is __cancelled__).
         
         Tips:
         * consider using HttpClient.GetAsync (as it is cancellable)
         * consider using Task.WhenAny
         * you may use urls like for testing https://postman-echo.com/delay/3
         * you should have problem with tasks cancellation -
            - how to merge tokens of operations (timeouts) with the provided token? 
            - Tip: you can link tokens with the help of CancellationTokenSource.CreateLinkedTokenSource(token)
         */
        public static async Task<string> ConcurrentDownloadAsync(this HttpClient httpClient,
            string[] urls, int millisecondsTimeout, CancellationToken token)
        {
            using CancellationTokenSource cts =
                CancellationTokenSource.CreateLinkedTokenSource(token);
            cts.CancelAfter(millisecondsTimeout);
            var tasks = urls.Select(url => httpClient.GetAsync(url, cts.Token));
            var whenAnyTask = await Task.WhenAny(tasks);
            cts.Cancel();
            var message = await whenAnyTask;
            var result = await message.Content.ReadAsStringAsync();
            return result;
        }

        public static async Task<string> ConcurrentDownloadAsync_Approach1(this HttpClient httpClient,
            string[] urls, int millisecondsTimeout, CancellationToken token)
        {
            var urlTasks = urls.Select(url => httpClient.GetAsync(url));
            var urlTask = Task.WhenAny(urlTasks);
            var timeoutTask = Task.Delay(millisecondsTimeout, token);
            var resultTask = Task.WhenAny(timeoutTask, urlTask);
            if (resultTask == timeoutTask)
            {
                throw new TaskCanceledException();
            }
            else
            {
                var urlResult = await urlTask;
                var message = await urlResult;
                var result = await message.Content.ReadAsStringAsync();
                return result;
            }
        }
    }
    

}
