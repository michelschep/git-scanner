using System;

namespace GitScannerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var gitScanner = new GitScanner();

            var root = @"C:\data\projects-michel";

            if (args.Length != 0)
                root = args[0];

            gitScanner.Scan(root);

            Console.ReadLine();
        }
    }
}
