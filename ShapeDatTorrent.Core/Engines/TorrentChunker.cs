using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BencodeNET.Parsing;
using BencodeNET.Torrents;

namespace ShapeDatTorrent.Core.Engines
{
    public class TorrentChunker
    {
        public event Action<string, ConsoleColor> OnLogMessage;

        public void Process(string torrentPath, string datPath, long targetBytes, List<string> regionsList)
        {
            var parser = new BencodeParser();
            Log("[..] Parsing master torrent metadata...");
            Torrent masterTorrent = parser.Parse<Torrent>(torrentPath);

            if (masterTorrent.Files == null)
            {
                Log("[ERROR] This application only supports multi-file torrent collections.", ConsoleColor.Red);
                return;
            }

            // Mode 1: DAT Curation
            if (!string.IsNullOrEmpty(datPath))
            {
                Log($"[MODE] DAT-Driven Curation Active!", ConsoleColor.Green);
                HashSet<string> allowedRomNames = ParseDatRomNames(datPath);
                List<MultiFileInfo> selectedFiles = new List<MultiFileInfo>();

                foreach (var file in masterTorrent.Files)
                {
                    string filename = file.Path.LastOrDefault();
                    if (filename != null && allowedRomNames.Contains(filename))
                        selectedFiles.Add(file);
                }

                if (selectedFiles.Count > 0)
                {
                    WriteFlatBatches(masterTorrent, selectedFiles, targetBytes, torrentPath);
                }
                else
                {
                    Log("[ERROR] Output halted: Zero files qualified.", ConsoleColor.Red);
                }
            }
            // Mode 2: Pure Swarm Balancer
            else
            {
                Log($"[MODE] Pure Swarm Balancing Engine Active\n", ConsoleColor.Cyan);
                Log("Building optimally sized region buckets...");
                Log("-----------------------------------------------------------------------");

                var regionalBuckets = regionsList.ToDictionary(r => r, r => new List<MultiFileInfo>(), StringComparer.OrdinalIgnoreCase);
                var unassignedFiles = new List<MultiFileInfo>();

                foreach (var file in masterTorrent.Files)
                {
                    string filename = file.Path.LastOrDefault() ?? "";
                    string matchedRegion = null;

                    foreach (var r in regionsList)
                    {
                        if (Regex.IsMatch(filename, @"\(" + Regex.Escape(r) + @"(\)|,)", RegexOptions.IgnoreCase))
                        {
                            matchedRegion = r;
                            break;
                        }
                    }

                    if (matchedRegion != null)
                        regionalBuckets[matchedRegion].Add(file);
                    else
                        unassignedFiles.Add(file);
                }

                // Dropout Check
                if (unassignedFiles.Count > 0)
                {
                    Log($"\n[WARNING] {unassignedFiles.Count} file(s) failed categorization and were sent to the dropout batch:", ConsoleColor.Red);
                    foreach (var file in unassignedFiles.Take(10))
                    {
                        Log($"  -> {file.Path.LastOrDefault() ?? "Unknown_File"}", ConsoleColor.DarkRed);
                    }
                    if (unassignedFiles.Count > 10)
                    {
                        Log($"  -> ... and {unassignedFiles.Count - 10} more.", ConsoleColor.DarkRed);
                    }

                    Log("\nThese have been safely bundled into an 'Unknown' batch.\n", ConsoleColor.Yellow);

                    if (!regionalBuckets.ContainsKey("Unknown"))
                        regionalBuckets["Unknown"] = new List<MultiFileInfo>();
                    regionalBuckets["Unknown"].AddRange(unassignedFiles);
                }

                var activeRegions = regionalBuckets
                    .Where(b => b.Value.Count > 0)
                    .Select(b => new { Name = b.Key, Files = b.Value, TotalBytes = b.Value.Sum(f => f.FileSize) })
                    .OrderByDescending(r => r.TotalBytes)
                    .ToList();

                long targetGB = targetBytes / (1024 * 1024 * 1024);
                Log($"### OPTIMIZED BATCH ALLOCATIONS (Target: {targetGB} GB)", ConsoleColor.Yellow);

                int batchIndex = 1;
                long currentBatchBytes = 0;
                var currentBatchFiles = new List<MultiFileInfo>();
                var currentBatchRegions = new List<string>();

                foreach (var region in activeRegions)
                {
                    // Case A: OVERSIZED REGION DETECTED -> Split into evenly distributed sub-chunks
                    if (region.TotalBytes > targetBytes)
                    {
                        if (currentBatchFiles.Count > 0)
                        {
                            string flushSuffix = GetBestBatchSuffix(currentBatchRegions);
                            SaveSubTorrent(masterTorrent, currentBatchFiles, batchIndex, torrentPath, flushSuffix);
                            batchIndex++;
                            currentBatchFiles.Clear();
                            currentBatchRegions.Clear();
                            currentBatchBytes = 0;
                        }

                        int partsNeeded = (int)Math.Ceiling((double)region.TotalBytes / targetBytes);
                        var sortedRegionFiles = region.Files.OrderByDescending(f => f.FileSize).ToList();

                        List<MultiFileInfo>[] splitBuckets = new List<MultiFileInfo>[partsNeeded];
                        for (int i = 0; i < partsNeeded; i++) splitBuckets[i] = new List<MultiFileInfo>();

                        int bucketAssignIdx = 0;
                        foreach (var file in sortedRegionFiles)
                        {
                            splitBuckets[bucketAssignIdx].Add(file);
                            bucketAssignIdx = (bucketAssignIdx + 1) % partsNeeded;
                        }

                        for (int i = 0; i < partsNeeded; i++)
                        {
                            string splitSuffix = $"{region.Name}-{i + 1}";
                            SaveSubTorrent(masterTorrent, splitBuckets[i], batchIndex, torrentPath, splitSuffix);
                            batchIndex++;
                        }
                        continue;
                    }

                    // Case B: Adding this region rolls over current batch target limit
                    if (currentBatchBytes + region.TotalBytes > targetBytes && currentBatchFiles.Count > 0)
                    {
                        string normalSuffix = GetBestBatchSuffix(currentBatchRegions);
                        SaveSubTorrent(masterTorrent, currentBatchFiles, batchIndex, torrentPath, normalSuffix);
                        batchIndex++;
                        currentBatchFiles.Clear();
                        currentBatchRegions.Clear();
                        currentBatchBytes = 0;
                    }

                    // Accumulate normally
                    currentBatchFiles.AddRange(region.Files);
                    currentBatchRegions.Add(region.Name);
                    currentBatchBytes += region.TotalBytes;
                }

                // Flush out remaining items
                if (currentBatchFiles.Count > 0)
                {
                    string finalSuffix = GetBestBatchSuffix(currentBatchRegions);
                    SaveSubTorrent(masterTorrent, currentBatchFiles, batchIndex, torrentPath, finalSuffix);
                }

                Log("-----------------------------------------------------------------------");
                Log("Analysis Complete. Copy your checkboxes directly out of the terminal.", ConsoleColor.Green);
            }
        }

        // Smart naming selector to pull out dominant regions or fallback to Various
        private string GetBestBatchSuffix(List<string> regionsInBatch)
        {
            if (regionsInBatch.Count == 1) return regionsInBatch[0];

            // If a collection contains a primary core region, name it after that region
            if (regionsInBatch.Contains("USA", StringComparer.OrdinalIgnoreCase)) return "USA";
            if (regionsInBatch.Contains("Europe", StringComparer.OrdinalIgnoreCase)) return "Europe";
            if (regionsInBatch.Contains("Japan", StringComparer.OrdinalIgnoreCase)) return "Japan";

            return "Various";
        }

        private void WriteFlatBatches(Torrent masterTorrent, List<MultiFileInfo> files, long targetBytes, string originalTorrentPath)
        {
            int batchIndex = 1;
            long currentBatchBytes = 0;
            var currentBatchFiles = new List<MultiFileInfo>();
            var sortedFiles = files.OrderByDescending(f => f.FileSize).ToList();

            foreach (var file in sortedFiles)
            {
                if (currentBatchBytes + file.FileSize > targetBytes && currentBatchFiles.Count > 0)
                {
                    SaveSubTorrent(masterTorrent, currentBatchFiles, batchIndex, originalTorrentPath, $"Batch-{batchIndex}");
                    batchIndex++;
                    currentBatchFiles.Clear();
                    currentBatchBytes = 0;
                }
                currentBatchFiles.Add(file);
                currentBatchBytes += file.FileSize;
            }

            if (currentBatchFiles.Count > 0)
                SaveSubTorrent(masterTorrent, currentBatchFiles, batchIndex, originalTorrentPath, $"Batch-{batchIndex}");
        }

        private void SaveSubTorrent(Torrent master, List<MultiFileInfo> batchFiles, int index, string originalPath, string nameSuffix)
        {
            var subTorrent = new Torrent
            {
                Trackers = master.Trackers,
                PieceSize = master.PieceSize,
                Pieces = master.Pieces,
                IsPrivate = master.IsPrivate,
                Encoding = master.Encoding,
                Comment = $"Balanced Section Chunk Batch {index}",
                CreatedBy = "ShapeDatTorrent Engine"
            };

            string rootDirectoryName = master.DisplayName ?? "DownloadBatch";
            var fileListContainer = new MultiFileInfoList(rootDirectoryName);

            foreach (var file in batchFiles)
            {
                fileListContainer.Add(file);
            }
            subTorrent.Files = fileListContainer;

            string outFileName = $"{Path.GetFileNameWithoutExtension(originalPath)}_{nameSuffix}.torrent";
            string outPath = Path.Combine(Path.GetDirectoryName(originalPath), outFileName);

            using (var fileStream = File.Create(outPath))
            {
                subTorrent.EncodeTo(fileStream);
            }

            double totalGB = Math.Round((double)batchFiles.Sum(f => f.FileSize) / (1024 * 1024 * 1024), 2);
            Log($" - [ ] Generated Batch {index}: {outFileName} ({totalGB} GB)", ConsoleColor.Gray);
        }

        private HashSet<string> ParseDatRomNames(string datPath)
        {
            var romNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var reader = XmlReader.Create(datPath))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "rom")
                    {
                        string nameAttribute = reader.GetAttribute("name");
                        if (!string.IsNullOrEmpty(nameAttribute)) romNames.Add(nameAttribute);
                    }
                }
            }
            return romNames;
        }

        private void Log(string message, ConsoleColor color = ConsoleColor.Gray)
        {
            OnLogMessage?.Invoke(message, color);
        }
    }
}