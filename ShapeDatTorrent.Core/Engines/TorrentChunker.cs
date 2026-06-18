using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BencodeNET.Parsing;
using BencodeNET.Torrents;
using ShapeDatTorrent.Core.Models;

namespace ShapeDatTorrent.Core.Engines
{
    public class TorrentChunker
    {
        public event Action<string, ConsoleColor> OnLogMessage;

        public void Process(string torrentPath, string keptDatPath, string removedDatPath, long targetBytes, List<string> regionsList, string outputDir)
        {
            var parser = new BencodeParser();
            Log("[..] Parsing master torrent metadata...");

            Torrent masterTorrent;
            try { masterTorrent = parser.Parse<Torrent>(torrentPath); }
            catch (Exception)
            {
                Log("[ERROR] Failed to parse torrent metadata.", ConsoleColor.Red);
                return;
            }

            if (masterTorrent.Files == null) return;
            int totalOriginalFiles = masterTorrent.Files.Count;

            // Mode 1: DAT-Driven Curation / Audit
            if (!string.IsNullOrEmpty(keptDatPath))
            {
                string mode = string.IsNullOrEmpty(removedDatPath) ? "Single DAT Mode" : "Dual DAT Mode";
                Log($"[MODE] DAT-Driven Curation Active ({mode})", ConsoleColor.Green);

                var registry = BuildAuditRegistry(keptDatPath, removedDatPath);
                List<MultiFileInfo> selectedFiles = new List<MultiFileInfo>();
                var auditEntries = new List<(string Type, string Filename, string Reason)>();

                int orphanCount = 0;
                int removalCount = 0;
                int includedCount = 0;

                foreach (var file in masterTorrent.Files)
                {
                    string filename = file.Path.LastOrDefault() ?? "";
                    string nameKey = Path.GetFileNameWithoutExtension(filename);

                    if (registry.TryGetValue(nameKey, out var entry))
                    {
                        if (entry.IsKept)
                        {
                            includedCount++;
                            selectedFiles.Add(file);
                            auditEntries.Add(("INCLUDED", filename, "Retained (Found in DAT)"));
                        }
                        else
                        {
                            removalCount++;
                            string cleanReason = entry.Reason.Replace("Remove reason:", "", StringComparison.OrdinalIgnoreCase).Trim();
                            string reasonText = $"Retool: Remove reason: {cleanReason}";
                            auditEntries.Add(("EXCLUDED", filename, reasonText));
                        }
                    }
                    else
                    {
                        orphanCount++;
                        string reasonText = string.IsNullOrEmpty(removedDatPath)
                            ? "Retool: Not in Dat. No Retool Removed dat sent."
                            : "Orphan (Not in any DAT)";

                        auditEntries.Add(("ORPHAN", filename, reasonText));
                    }
                }

                // Report Statistics
                Log($"[STATS] Audit complete. Orphans: {orphanCount} | Removals: {removalCount} | Included: {includedCount}", ConsoleColor.Yellow);

                // Sort: ORPHAN (0) -> EXCLUDED (1) -> INCLUDED (2), then alphabetical by filename
                var finalLogLines = auditEntries
                    .OrderBy(x => x.Type == "ORPHAN" ? 0 : (x.Type == "EXCLUDED" ? 1 : 2))
                    .ThenBy(x => x.Filename)
                    .Select(x => $"{x.Type} | File: {x.Filename} | {x.Reason}");

                string logPath = Path.Combine(outputDir, "analysis_log.txt");
                File.WriteAllLines(logPath, finalLogLines);

                Log($"[REPORT] Audit log written to: {logPath}", ConsoleColor.DarkGray);

                if (selectedFiles.Count > 0)
                    WriteFlatBatches(masterTorrent, selectedFiles, targetBytes, torrentPath, totalOriginalFiles, outputDir, true);
                else
                    Log("[ERROR] Output halted: Zero files qualified.", ConsoleColor.Red);
            }

            // Mode 2: Pure Swarm Balancer
            else
            {
                Log($"[MODE] Pure Swarm Balancing Engine Active!", ConsoleColor.Cyan);
                Log($"[PATH] Target Output Folder: \"{outputDir}\"\n", ConsoleColor.White);

                var regionalBuckets = regionsList.ToDictionary(r => r, r => new List<MultiFileInfo>(), StringComparer.OrdinalIgnoreCase);
                var unassignedFiles = new List<MultiFileInfo>();

                foreach (var file in masterTorrent.Files)
                {
                    string filename = file.Path.LastOrDefault() ?? "";
                    string matchedRegion = regionsList.FirstOrDefault(r => Regex.IsMatch(filename, @"\(" + Regex.Escape(r) + @"(\)|,)", RegexOptions.IgnoreCase));

                    if (matchedRegion != null) regionalBuckets[matchedRegion].Add(file);
                    else unassignedFiles.Add(file);
                }

                if (unassignedFiles.Count > 0)
                {
                    Log($"\n[WARNING] {unassignedFiles.Count} file(s) failed categorization.", ConsoleColor.Red);
                    if (!regionalBuckets.ContainsKey("Unknown")) regionalBuckets["Unknown"] = new List<MultiFileInfo>();
                    regionalBuckets["Unknown"].AddRange(unassignedFiles);
                }

                var activeRegions = regionalBuckets.Where(b => b.Value.Count > 0)
                    .Select(b => new { Name = b.Key, Files = b.Value, TotalBytes = b.Value.Sum(f => f.FileSize) })
                    .OrderByDescending(r => r.TotalBytes).ToList();

                int batchIndex = 1;
                long currentBatchBytes = 0;
                var currentBatchFiles = new List<MultiFileInfo>();
                var currentBatchRegions = new List<string>();

                foreach (var region in activeRegions)
                {
                    if (region.TotalBytes > targetBytes)
                    {
                        if (currentBatchFiles.Count > 0)
                        {
                            SaveSubTorrent(masterTorrent, currentBatchFiles, batchIndex, torrentPath, GetBestBatchSuffix(currentBatchRegions), totalOriginalFiles, outputDir);
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
                            SaveSubTorrent(masterTorrent, splitBuckets[i], batchIndex, torrentPath, $"{region.Name}-{i + 1}", totalOriginalFiles, outputDir);
                            batchIndex++;
                        }
                        continue;
                    }

                    if (currentBatchBytes + region.TotalBytes > targetBytes && currentBatchFiles.Count > 0)
                    {
                        SaveSubTorrent(masterTorrent, currentBatchFiles, batchIndex, torrentPath, GetBestBatchSuffix(currentBatchRegions), totalOriginalFiles, outputDir);
                        batchIndex++;
                        currentBatchFiles.Clear();
                        currentBatchRegions.Clear();
                        currentBatchBytes = 0;
                    }

                    currentBatchFiles.AddRange(region.Files);
                    currentBatchRegions.Add(region.Name);
                    currentBatchBytes += region.TotalBytes;
                }

                if (currentBatchFiles.Count > 0)
                    SaveSubTorrent(masterTorrent, currentBatchFiles, batchIndex, torrentPath, GetBestBatchSuffix(currentBatchRegions), totalOriginalFiles, outputDir);
            }
        }

        // Update the registry definition to hold a list of potential candidates
        private Dictionary<string, List<AuditEntry>> _registry = new Dictionary<string, List<AuditEntry>>(StringComparer.OrdinalIgnoreCase);

        private void BuildAuditRegistry(string keptPath, string removedPath)
        {
            _registry.Clear();
            // ParseDat now appends to the list
            ParseDat(keptPath, true);
            if (!string.IsNullOrEmpty(removedPath)) ParseDat(removedPath, false);
        }

        private string GetBestBatchSuffix(List<string> regionsInBatch)
        {
            if (regionsInBatch.Count == 1) return regionsInBatch[0];
            if (regionsInBatch.Contains("USA", StringComparer.OrdinalIgnoreCase)) return "USA";
            if (regionsInBatch.Contains("Europe", StringComparer.OrdinalIgnoreCase)) return "Europe";
            if (regionsInBatch.Contains("Japan", StringComparer.OrdinalIgnoreCase)) return "Japan";
            return "Various";
        }

        private void Log(string message, ConsoleColor color = ConsoleColor.Gray) => OnLogMessage?.Invoke(message, color);

        private void ParseDat(string path, bool isKept)
        {
            var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore, XmlResolver = null };
            using (var reader = XmlReader.Create(path, settings))
            {
                string currentGame = null;
                string currentComment = null;
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "game")
                        currentGame = reader.GetAttribute("name");

                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "comment" && !isKept)
                        currentComment = reader.ReadElementContentAsString();

                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "game" && currentGame != null)
                    {
                        // Extract clean name (e.g., "Dynamite Soccer 98")
                        string cleanName = GetCleanName(currentGame);

                        if (!_registry.ContainsKey(cleanName))
                            _registry[cleanName] = new List<AuditEntry>();

                        _registry[cleanName].Add(new AuditEntry { Name = currentGame, IsKept = isKept, Reason = isKept ? "Retained" : (currentComment ?? "Excluded") });

                        currentGame = null; currentComment = null;
                    }
                }
            }
        }

        private void SaveSubTorrent(Torrent master, List<MultiFileInfo> batchFiles, int index, string originalPath, string nameSuffix, int totalOriginalFiles, string outputDir)
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

            var fileListContainer = new MultiFileInfoList(master.DisplayName ?? "DownloadBatch");
            foreach (var file in batchFiles) fileListContainer.Add(file);
            subTorrent.Files = fileListContainer;

            if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);

            string outPath = Path.Combine(outputDir, $"{Path.GetFileNameWithoutExtension(originalPath)}_{nameSuffix}.torrent");
            using (var fileStream = File.Create(outPath)) subTorrent.EncodeTo(fileStream);

            Log($" - [ ] Generated Batch {index}: {Path.GetFileName(outPath)} ({Math.Round((double)batchFiles.Sum(f => f.FileSize) / (1024 * 1024 * 1024), 2)} GB) | Files: {batchFiles.Count}/{totalOriginalFiles}", ConsoleColor.Gray);
        }

        private void WriteFlatBatches(Torrent masterTorrent, List<MultiFileInfo> files, long targetBytes, string originalTorrentPath, int totalOriginalFiles, string outputDir, bool isCurated)
        {
            int batchIndex = 1;
            long currentBatchBytes = 0;
            var currentBatchFiles = new List<MultiFileInfo>();
            var sortedFiles = files.OrderByDescending(f => f.FileSize).ToList();

            foreach (var file in sortedFiles)
            {
                if (currentBatchBytes + file.FileSize > targetBytes && currentBatchFiles.Count > 0)
                {
                    string suffix = isCurated ? $"Batch-Curated-{batchIndex}" : $"Batch-{batchIndex}";
                    SaveSubTorrent(masterTorrent, currentBatchFiles, batchIndex, originalTorrentPath, suffix, totalOriginalFiles, outputDir);
                    batchIndex++;
                    currentBatchFiles.Clear();
                    currentBatchBytes = 0;
                }
                currentBatchFiles.Add(file);
                currentBatchBytes += file.FileSize;
            }

            if (currentBatchFiles.Count > 0)
            {
                string suffix = isCurated ? $"Batch-Curated-{batchIndex}" : $"Batch-{batchIndex}";
                SaveSubTorrent(masterTorrent, currentBatchFiles, batchIndex, originalTorrentPath, suffix, totalOriginalFiles, outputDir);
            }
        }
    }
}