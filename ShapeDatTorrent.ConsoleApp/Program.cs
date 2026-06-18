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

            Console.Write("Enter target chunk batch size in GB: ");
            if (!double.TryParse(Console.ReadLine(), out double targetGB)) targetGB = 1000;
            long targetBytes = (long)(targetGB * 1024 * 1024 * 1024);

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