using System;
using System.Threading;
using Synchronization.Core;

namespace Synchronization
{
    class Program
    {
        static void Main(string[] args)
        {
            var scopeName = "default";
            var isSystemWide = false;
            if (args.Length == 2)
            {
                scopeName = args[0];
                isSystemWide = bool.Parse(args[1]);
            }
            using (new NamedExclusiveScope(scopeName, isSystemWide))
            {
                Console.WriteLine("Hello world!");
                Thread.Sleep(300);
            }
        }
    }
}
