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

            // 0. Handle Help and Empty Args
            var helpFlags = new[] { "-h", "-help", "--help" };
            if (args.Length == 0 || args.Any(a => helpFlags.Contains(a, StringComparer.OrdinalIgnoreCase)))
            {
                if (args.Length == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ERROR] No arguments provided.");
                    Console.ResetColor();
                }
                ShowHelp();
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                return;
            }

            // Capture the strict flag
            bool strict = args.Contains("--strict", StringComparer.OrdinalIgnoreCase);

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
                f != "--verbose" &&
                f != "--strict"); // Ensure we don't treat the flag as an output directory

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
                    "USA", "Europe", "World", "United Kingdom", "UK", "Germany", "France", "Spain", "Italy", "Australia", "Japan", "Korea", "China",
                    "Canada", "Brazil", "Netherlands", "Sweden", "Norway", "Denmark", "Finland", "Portugal", "Russia", "Asia", "Taiwan",
                    "Greece", "Israel", "Belgium", "Austria", "Ireland", "Poland", "Scandinavia", "Argentina", "Switzerland", "Croatia",
                    "Czech", "Hungary", "Iceland", "India", "New Zealand", "Singapore", "Slovakia", "South Africa", "Turkey", "Ukraine",
                    "Export", "Hong Kong", "Thailand", "Latin America", "North America", "South America", "Central America", "Middle East",
                    "Oceania", "Africa", "Unknown", "Soviet Union", "USSR", "Yugoslavia", "Bulgaria", "Romania", "Luxembourg", "Cyprus",
                    "Malta", "Slovenia", "Estonia", "Latvia", "Lithuania", "Mexico", "Chile", "Colombia", "Peru", "Venezuela", "Philippines",
                    "Malaysia", "Indonesia", "Vietnam", "Saudi Arabia", "UAE", "Egypt", "Morocco", "Tunisia"
                };

            // Pass 'strict' into the Process method
            chunker.Process(torrentPath, keptDatPath, removedDatPath, targetBytes, regions, outputDir, strict);

            Console.WriteLine("\nDone. Press any key to exit...");
            Console.ReadKey();
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Separates multi-file, large download footprint torrents into user-set size individual torrents");
            Console.WriteLine("E.g. a 10,000 file, 3TB rom torrent into 3 x 1TB ones");
            Console.WriteLine("Additional features:");
            Console.WriteLine("Curation: Include a Dat file (e.g. a ReTool one) to curate which files are placed in the final torrents");
            Console.WriteLine("Reporting: Include an additional dat file of excluded files for full report of Orphans, Excluded and Included files");
            Console.WriteLine("\nUsage: ShapeDatTorrent.exe <.torrent path> [options]");
            Console.WriteLine("       Drag and Drop files onto the exe (auto-detects file types)");
            Console.WriteLine("\nRequired:");
            Console.WriteLine("  [file].torrent      Path to the source .torrent file.");
            Console.WriteLine("\nOptional:");
            Console.WriteLine("  [file].dat          Path to DAT file(s) for curation.");
            Console.WriteLine("  [directory]         Custom output folder path.");
            Console.WriteLine("\nFlags:");
            Console.WriteLine("  --strict            Enable strict matching (Default: Loose matching).");
            Console.WriteLine("\nExamples:");
            Console.WriteLine("  ShapeDatTorrent.exe mycollection.torrent games.dat");
            Console.WriteLine("  ShapeDatTorrent.exe mycollection.torrent kept.dat removed.dat C:\\Output --strict");
        }
        private static void IdentifyDats(List<string> datPaths, out string kept, out string removed)
        {
            kept = null;
            removed = null;
            foreach (var path in datPaths)
            {
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
                catch { }

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