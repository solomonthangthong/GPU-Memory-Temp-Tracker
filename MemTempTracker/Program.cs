using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Text.Json;

namespace MemTempTracker
{
    internal class Program
    {
        // Hardcoded path, update to your log file location
        static string logFilePath = @"C:\Users\mon\Documents\GPUZ\Log.txt";
        static string targetProcess = "Salad";
        static char delimiter = ',';
        static int tempThreshold = 100;

        // Parse Log and return temperature value
        static double? GetLastestMemoryTemperature() 
        {
            if (!File.Exists(logFilePath))
            {
                Console.WriteLine("GPU-Z log file not found.");
                return null;
            }

            try
            {
                // Open the file with shared access for read and write 
                using (FileStream fileStream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    // Latest reading is at the bottom, so read from reverse
                    var lines = reader.ReadToEnd()
                              .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries) // Remove empty lines
                              .Reverse()
                              .ToList();

                    if (lines.Count < 2)
                    {
                        Console.WriteLine("Not enough lines in the log file.");
                        return null;
                    }

                    //Retrieve first line in the file, split by comma, h => h remove extra spaces, and return into Array
                    // Date, GPU Clock [MHz], Memory Temperature [°C], Fan Speed [%] to ["Date", "GPU Clock [MHz]", "Memory Temperature [°C]", "Fan Speed [%]"]
                    var header = lines.Last().Split(',').Select(h => h.Trim()).ToArray();

                    // Get last line in file, split into array by comma, remove extra spaces
                    // 2025-02-06 18:04:10, 210.0, 36.0, 20 to ["2025-02-06 18:04:10", "210.0", "36.0", "20"]
                    var latestData = lines.First().Split(',').Select(d => d.Trim()).ToArray(); // Latest entry

                    // Ensure header and latestData have the same number of columns
                    if (latestData.Length < header.Length)
                    {
                        Console.WriteLine($"Mismatch: Header has {header.Length} columns, but latestData has {latestData.Length} columns.");
                        return null;
                    }

                    int memoryTempIndex = Array.FindIndex(header, col => col.Contains("Memory Temperature", StringComparison.OrdinalIgnoreCase));

                    if (memoryTempIndex == -1 || memoryTempIndex >= latestData.Length)
                    {
                        Console.WriteLine("Memory Temperature column NOT found or index is out of range.");
                        return null;
                    }
                   
                    // out double temp store the parsed number when TryParse successfully converts string into double
                    if (double.TryParse(latestData[memoryTempIndex], out double temp))
                    {
                        return temp;
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading log: {ex.Message}");
            }

            return null;
        }

        // Terminate Salad program
        static void KillProcess(string processName) 
        {
            var processes = Process.GetProcessesByName(processName);
            foreach (var process in processes)
            {
                try {
                    process.Kill();
                    Console.WriteLine($"{processName} has been terminated.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to terminate {processName}: {ex.Message}");
                }
            }
        }


        static void Main(string[] args)
        {
            Console.Write("Monitoring GPU-Z logs for VRAM temperature...\n");

            while (true)
            {
                try
                {
                    double? latestTemp = GetLastestMemoryTemperature();


                    if (latestTemp.HasValue) 
                    {
                        // Reset cursor to start of the line
                        Console.Write("\rTemperature Reading: ");

                        // Colour format for text printed after this line
                        if (latestTemp < 80)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        else if (latestTemp > 80 && latestTemp < 100)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }
                        else 
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }

                        Console.Write($"{latestTemp}°C ");

                        // Reset console color to default
                        // Stops from applying the colour to any new text print after this point
                        Console.ResetColor();

                        if (latestTemp >= tempThreshold) 
                        {
                            Console.WriteLine($"Temperature exceeded {tempThreshold}°C! Terminating {targetProcess}");
                            KillProcess(targetProcess);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No valid temperature reading found.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                Thread.Sleep(5000); // Check every 5 seconds
            }
        }
    }
}
