using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;

namespace MemTempTracker
{
    internal class Program
    {
        // Hardcoded path, update to your log file location
        static string logFilePath = @"C:\Users\mon\Documents\GPUZ\Log.txt";
        static string targetProcess = "Salad";
        static char delimiter = ',';
        static int tempThreshold = 100;
        static void Main(string[] args)
        {
            Console.WriteLine("Monitoring GPU-Z logs for VRAM temperature...");
        }
    }
}
