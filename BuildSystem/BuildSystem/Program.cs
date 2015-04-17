using System;

namespace BuildSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildSystemApp app = new BuildSystemApp();
            
            Environment.ExitCode = app.Start(args);
            if (Environment.ExitCode == 1)
            {
                PrintHelp();
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Build system for Windows 8.1 applications.\nUsage:\nBuildSystem <catalog_id>");
        }
    }
}
