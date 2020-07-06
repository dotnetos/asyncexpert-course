using System;
using System.Diagnostics;

namespace TaskCompletionSourceExercises
{
    class Program
    {
        static void Main(string[] args)
        {
            ////////////////////////////////////////////////////////////////////////////////
            // Here is an example program presenting Process API to be used in the exercise.

            var process = new Process();
            process.EnableRaisingEvents = true;
            process.StartInfo = new ProcessStartInfo(@"..\..\..\..\..\ExampleApp\bin\x64\Debug\netcoreapp3.1\ExampleApp.exe")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            process.Exited += (sender, eventArgs) =>
            {
                var senderProcess = sender as Process;
                Console.WriteLine("----- program output -----");
                Console.WriteLine($"Exit code: {senderProcess?.ExitCode}");
                Console.WriteLine("Standard output:");
                Console.Write(senderProcess?.StandardOutput.ReadToEnd());
                Console.WriteLine("Standard error:");
                Console.Write(senderProcess?.StandardError.ReadToEnd());
                Console.WriteLine("----- program output -----");
                senderProcess?.Dispose();
            };
            process.Start();
            Console.WriteLine("Test");
            Console.ReadKey();
        }
    }
}
