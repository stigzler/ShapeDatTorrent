using BencodeNET.Parsing;
using BencodeNET.Torrents;
using ShapeDatTorrent.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ShapeDatTorrent.Core.Engines
{
    public class TorrentChunker
    {
        public event Action<string, ConsoleColor> OnLogMessage;

        private Dictionary<string, List<AuditEntry>> _looseRegistry = new Dictionary<string, List<AuditEntry>>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, AuditEntry> _strictRegistry = new Dictionary<string, AuditEntry>(StringComparer.OrdinalIgnoreCase);


        public void Process(string torrentPath, string keptDatPath, string removedDatPath,
            long targetBytes, List<string> regionsList, string outputDir, bool strict)
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

            if (!string.IsNullOrEmpty(keptDatPath))
            {
                string mode = string.IsNullOrEmpty(removedDatPath) ? "Single DAT Mode" : "Dual DAT Mode";
                Log($"[MODE] DAT-Driven Curation Active ({mode}) | Strict Mode: {strict}", ConsoleColor.Green);

                BuildAuditRegistry(keptDatPath, removedDatPath);
                List<MultiFileInfo> selectedFiles = new List<MultiFileInfo>();
                var auditEntries = new List<(string Type, string Filename, string Reason)>();

                int orphanCount = 0, removalCount = 0, includedCount = 0;

                foreach (var file in masterTorrent.Files)
                {
                    string filename = file.Path.LastOrDefault() ?? "";
                    string nameKey = Path.GetFileNameWithoutExtension(filename);
                    AuditEntry match = null;

                    if (strict)
                    {
                        // Strict Match: Look in the Exact Name registry
                        if (_strictRegistry.TryGetValue(nameKey, out var exactMatch))
                            match = exactMatch;
                    }
                    else
                    {
                        // Loose Match: Look in the Cleaned Name registry
                        string cleanName = GetCleanName(nameKey);
                        if (_looseRegistry.TryGetValue(cleanName, out var candidates))
                            match = FindBestMatch(nameKey, candidates, regionsList);
                    }

                    if (match != null)
                    {
                        if (match.IsKept)
                        {
                            includedCount++;
                            selectedFiles.Add(file);
                            auditEntries.Add(("INCLUDED", filename, "Retained (Found in DAT)"));
                        }
                        else
                        {
                            removalCount++;
                            string cleanReason = match.Reason.Replace("Remove reason:", "", StringComparison.OrdinalIgnoreCase).Trim();
                            auditEntries.Add(("EXCLUDED", filename, $"Retool: Remove reason: {cleanReason}"));
                        }
                    }
                    else
                    {
                        orphanCount++;
                        string reasonText = string.IsNullOrEmpty(removedDatPath) ? "Retool: Not in Dat. No Retool Removed dat sent." : "Orphan (Not in any DAT)";
                        auditEntries.Add(("ORPHAN", filename, reasonText));
                    }
                }

                Log($"[STATS] Audit complete. Orphans: {orphanCount} | Removals: {removalCount} | Included: {includedCount}", ConsoleColor.Yellow);

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

        private string GetCleanName(string filename)
        {
            var match = Regex.Match(filename, @"^(.*?)\s*\(", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value.Trim() : filename;
        }

        private AuditEntry FindBestMatch(string filename, List<AuditEntry> candidates, List<string> regionPriority)
        {
            if (candidates.Count == 1) return candidates[0];

            var fileRegions = Regex.Matches(filename, @"\((.*?)\)").Cast<Match>()
                                   .SelectMany(m => m.Groups[1].Value.Split(','))
                                   .Select(r => r.Trim()).ToList();

            return candidates.OrderBy(c => {
                var cRegions = Regex.Matches(c.Name, @"\((.*?)\)").Cast<Match>()
                                    .SelectMany(m => m.Groups[1].Value.Split(','))
                                    .Select(r => r.Trim()).ToList();

                var common = cRegions.Intersect(fileRegions, StringComparer.OrdinalIgnoreCase).ToList();
                return common.Any() ? common.Min(r => regionPriority.FindIndex(p => p.Equals(r, StringComparison.OrdinalIgnoreCase)) != -1
                                                     ? regionPriority.FindIndex(p => p.Equals(r, StringComparison.OrdinalIgnoreCase))
                                                     : 999)
                                    : 999;
            }).FirstOrDefault();
        }

        // Update the registry definition to hold a list of potential candidates
        private Dictionary<string, List<AuditEntry>> _registry = new Dictionary<string, List<AuditEntry>>(StringComparer.OrdinalIgnoreCase);

        private void BuildAuditRegistry(string keptPath, string removedPath)
        {
            _looseRegistry.Clear();
            _strictRegistry.Clear();
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
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "game")
                        {
                            currentGame = reader.GetAttribute("name");
                        }
                        else if (reader.Name == "comment" && !isKept)
                        {
                            currentComment = reader.ReadElementContentAsString();
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "game")
                    {
                        if (!string.IsNullOrEmpty(currentGame))
                        {
                            // Construct the entry
                            var entry = new AuditEntry
                            {
                                Name = currentGame,
                                IsKept = isKept,
                                Reason = isKept ? "Retained" : (currentComment ?? "Excluded")
                            };

                            // 1. Populate Strict Registry (Key is exact filename/DAT name)
                            _strictRegistry[currentGame] = entry;

                            // 2. Populate Loose Registry (Key is cleaned name for fuzzy matching)
                            string cleanName = GetCleanName(currentGame);
                            if (!_looseRegistry.ContainsKey(cleanName))
                            {
                                _looseRegistry[cleanName] = new List<AuditEntry>();
                            }
                            _looseRegistry[cleanName].Add(entry);
                        }

                        // Reset for next block
                        currentGame = null;
                        currentComment = null;
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