using System;
using System.Linq;
using System.Threading;

namespace ExampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                throw new Exception("Missing program argument.");
            }
            Console.WriteLine($"Hello {args.First()}!");
            Thread.Sleep(100);
            Console.WriteLine("Goodbye.");
        }
    }
}
