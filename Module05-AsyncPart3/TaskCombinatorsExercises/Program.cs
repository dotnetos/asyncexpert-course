using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TaskCombinatorsExercises.Core;

namespace TaskCombinatorsExercises
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var client = new HttpClient();
            Debug("Starting operations");
            var result = await client.ConcurrentDownloadAsync(new[]
            {
                "https://postman-echo.com/delay/7",
                "https://postman-echo.com/delay/6",
                "https://postman-echo.com/delay/3"
            }, 10_000, CancellationToken.None);
            Debug(result);
        }

        static void Debug(string label)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.ffff}] {label}");
        }
    }
}
