using ShapeDatTorrent.Core.Engines;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeDatTorrent.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "ShapeDatTorrent Shell";
            Console.WriteLine("=======================================================================");
            Console.WriteLine("                       SHAPE DAT TORRENT                               ");
            Console.WriteLine("=======================================================================");

            if (args.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] Please drop a .torrent (or a .dat + .torrent combo) onto this EXE.");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            string datPath = args.FirstOrDefault(f => f.EndsWith(".dat", StringComparison.OrdinalIgnoreCase));
            string torrentPath = args.FirstOrDefault(f => f.EndsWith(".torrent", StringComparison.OrdinalIgnoreCase));

            // 1. Check if a torrent file was actually provided
            if (string.IsNullOrEmpty(torrentPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] No .torrent file was provided. Please drop a valid .torrent file onto the EXE.");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            // 2. Verify the torrent file physically exists on disk
            if (!File.Exists(torrentPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] Target torrent file could not be found:\n        \"{torrentPath}\"");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            // 3. Optional: Verify the DAT file exists if one was provided
            if (!string.IsNullOrEmpty(datPath) && !File.Exists(datPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] Target DAT file was specified but could not be found:\n        \"{datPath}\"");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            long targetGB;
            while (true)
            {
                Console.Write("Enter target chunk batch size in GB (Minimum 50): ");
                string input = Console.ReadLine()?.Trim();

                // 1. Check if it's a valid whole number (this naturally rejects decimals)
                if (!long.TryParse(input, out targetGB))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ERROR] Input must be a valid whole number (no decimals or letters).\n");
                    Console.ResetColor();
                    continue;
                }

                // 2. Check lower boundary
                if (targetGB < 50)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ERROR] Batch size cannot be less than 50 GB.\n");
                    Console.ResetColor();
                    continue;
                }

                // 3. Prevent Byte Overflow (Additional Check)
                if (targetGB > 8589934591)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ERROR] Input is too large! The maximum allowable size is 8,589,934,591 GB.\n");
                    Console.ResetColor();
                    continue;
                }

                break; // Input is completely valid, break the loop
            }

            long targetBytes = targetGB * 1024 * 1024 * 1024;

            // Fire up our modular engine
            var chunker = new TorrentChunker();

            // Listen to messages coming from the Core project and write them out to the screen
            chunker.OnLogMessage += (msg, color) =>
            {
                Console.ForegroundColor = color;
                Console.WriteLine(msg);
                Console.ResetColor();
            };

            // Exhaustive 78-region array from your original batch script
            var regions = new List<string> {
                "USA", "Europe", "Japan", "World", "Germany", "France", "Spain", "Italy", "Australia", "Korea", "China", "Canada", "Brazil",
                "UK", "United Kingdom", "Netherlands", "Sweden", "Norway", "Denmark", "Finland", "Portugal", "Russia", "Asia", "Taiwan",
                "Greece", "Israel", "Belgium", "Austria", "Ireland", "Poland", "Scandinavia", "Argentina", "Switzerland", "Croatia",
                "Czech", "Hungary", "Iceland", "India", "New Zealand", "Singapore", "Slovakia", "South Africa", "Turkey", "Ukraine",
                "Export", "Hong Kong", "Thailand", "Latin America", "North America", "South America", "Central America", "Middle East",
                "Oceania", "Africa", "Unknown", "Soviet Union", "USSR", "Yugoslavia", "Bulgaria", "Romania", "Luxembourg", "Cyprus",
                "Malta", "Slovenia", "Estonia", "Latvia", "Lithuania", "Mexico", "Chile", "Colombia", "Peru", "Venezuela", "Philippines",
                "Malaysia", "Indonesia", "Vietnam", "Saudi Arabia", "UAE", "Egypt", "Morocco", "Tunisia"
            };

            chunker.Process(torrentPath, datPath, targetBytes, regions);

            Console.WriteLine("\nDone. Press any key to exit...");
            Console.ReadKey();
        }
    }
}