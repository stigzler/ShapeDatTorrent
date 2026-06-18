using ShapeDatTorrent.Core.Engines;
using System;
using System.Collections.Generic;
using System.IO;
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

            // 1. Organize Inputs
            string torrentPath = args.FirstOrDefault(f => f.EndsWith(".torrent", StringComparison.OrdinalIgnoreCase));
            var datFiles = args.Where(f => f.EndsWith(".dat", StringComparison.OrdinalIgnoreCase)).ToList();

            // 2. Identify and Validate DATs
            string keptDatPath = null;
            string removedDatPath = null;

            if (datFiles.Count > 0)
            {
                IdentifyDats(datFiles, out keptDatPath, out removedDatPath);
            }

            // 3. Validate mandatory Torrent file
            if (string.IsNullOrEmpty(torrentPath) || !File.Exists(torrentPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] No valid .torrent file was provided/found.");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            // 4. Check output folder
            string outputDir = args.FirstOrDefault(f =>
                !f.EndsWith(".dat", StringComparison.OrdinalIgnoreCase) &&
                !f.EndsWith(".torrent", StringComparison.OrdinalIgnoreCase) &&
                f != "--verbose");

            if (string.IsNullOrEmpty(outputDir))
                outputDir = AppDomain.CurrentDomain.BaseDirectory;
            else if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            // 5. Gather Batch Size
            long targetBytes = GetTargetBytes();

            // 6. Execute
            var chunker = new TorrentChunker();
            chunker.OnLogMessage += (msg, color) =>
            {
                Console.ForegroundColor = color;
                Console.WriteLine(msg);
                Console.ResetColor();
            };

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

            // Call the updated Process method
            chunker.Process(torrentPath, keptDatPath, removedDatPath, targetBytes, regions, outputDir);

            Console.WriteLine("\nDone. Press any key to exit...");
            Console.ReadKey();
        }

        private static void IdentifyDats(List<string> datPaths, out string kept, out string removed)
        {
            kept = null;
            removed = null;

            foreach (var path in datPaths)
            {
                // Peek at the beginning of the file to check for "Removed titles" string
                // Using a small buffer is safer for large files
                bool isRemovedDat = false;
                try
                {
                    using (var reader = new StreamReader(path))
                    {
                        string line;
                        int count = 0;
                        while ((line = reader.ReadLine()) != null && count < 10)
                        {
                            if (line.Contains("(Removed titles)", StringComparison.OrdinalIgnoreCase))
                            {
                                isRemovedDat = true;
                                break;
                            }
                            count++;
                        }
                    }
                }
                catch { /* Handle/Log if file is locked */ }

                if (isRemovedDat) removed = path;
                else kept = path;
            }
        }

        private static long GetTargetBytes()
        {
            long targetGB;
            while (true)
            {
                Console.Write("Enter target chunk batch size in GB (Minimum 50): ");
                string input = Console.ReadLine()?.Trim();
                if (long.TryParse(input, out targetGB) && targetGB >= 50)
                    return targetGB * 1024 * 1024 * 1024;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] Invalid size. Minimum 50GB required.");
                Console.ResetColor();
            }
        }
    }
}