using BencodeNET.Parsing;
using BencodeNET.Torrents;
using Microsoft.VisualBasic;
using ShapeDatTorrent.Core.DTOs;
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

    //    public void Process(string torrentPath, string keptDatPath, string removedDatPath,
    //long targetBytes, List<string> regionsList, string outputDir, bool strict)

        public void Process(ProcessingRequest request)
        {
            var parser = new BencodeParser();
            Log($"[{DateTime.Now.ToString("dd.MM.yy HH:mm.ss")}] Parsing master torrent metadata...", ConsoleColor.Green);

            Torrent masterTorrent;
            try { masterTorrent = parser.Parse<Torrent>(request.TorrentPath); }
            catch (Exception)
            {
                Log("[ERROR] Failed to parse torrent metadata.", ConsoleColor.Red);
                return;
            }

            if (masterTorrent.Files == null) return;
            int totalOriginalFiles = masterTorrent.Files.Count;

            // ====================================================================
            // Mode 1: Filter by Dat

            if (!string.IsNullOrEmpty(request.KeptDatPath))
            {
                string mode = string.IsNullOrEmpty(request.RemovedDatPath) ? "Just Filter" : "Also use Excluded";
                Log($"[MODE] DAT Filter Active ({mode}) | Strict Mode: {request.Strict}", ConsoleColor.Cyan);
                Log($"[PATH] Target Output Folder: \"{request.OutputDir}\"", ConsoleColor.Gray);

                BuildAuditRegistry(request.KeptDatPath, request.RemovedDatPath);
                List<MultiFileInfo> selectedFiles = new List<MultiFileInfo>();
                var auditEntries = new List<(string Type, string Filename, string Reason)>();

                int orphanCount = 0, removalCount = 0, includedCount = 0;

                foreach (var file in masterTorrent.Files)
                {
                    string filename = file.Path.LastOrDefault() ?? "";
                    string nameKey = Path.GetFileNameWithoutExtension(filename);
                    AuditEntry match = null;

                    if (request.Strict)
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
                            match = FindBestMatch(nameKey, candidates, Models.TorrentConstants.DefaultRegions);
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
                            auditEntries.Add(("EXCLUDED", filename, $"Filter dat removal reason: {cleanReason}"));
                        }
                    }
                    else
                    {
                        orphanCount++;
                        string reasonText = string.IsNullOrEmpty(request.RemovedDatPath) ? "Not in Filter .dat. No Excluded .dat sent." : "Orphan (Not in any DAT)";
                        auditEntries.Add(("ORPHAN", filename, reasonText));
                    }
                }

                Log($"[STATS] Audit complete. Orphans: {orphanCount} | Removals: {removalCount} | Included: {includedCount}", ConsoleColor.Yellow);

                var finalLogLines = auditEntries
                    .OrderBy(x => x.Type == "ORPHAN" ? 0 : (x.Type == "EXCLUDED" ? 1 : 2))
                    .ThenBy(x => x.Filename)
                    .Select(x => $"{x.Type} | File: {x.Filename} | {x.Reason}");

                string logPath = Path.Combine(request.OutputDir, "ShapeDatTorrent.log");
                File.WriteAllLines(logPath, finalLogLines);
                Log($"[REPORT] Audit log written to: {logPath}", ConsoleColor.Gray);

                if (selectedFiles.Count > 0)
                    WriteFlatBatches(masterTorrent, selectedFiles, request.TargetBytes, request.TorrentPath, totalOriginalFiles, request.OutputDir, true);
                else
                    Log("[ERROR] Output halted: Zero files qualified.", ConsoleColor.Red);
            }

            // ====================================================================
            // Mode 2: Simple Split
            else
            {
                Log($"[MODE] Simple .torrent split", ConsoleColor.Cyan);
                Log($"[PATH] Target Output Folder: \"{request.OutputDir}\"", ConsoleColor.Gray);
                Log($"[STATS] All files included given no .dat file. Included: {totalOriginalFiles}", ConsoleColor.Yellow);

                var regionalBuckets = Models.TorrentConstants.DefaultRegions.ToDictionary(r => r, r => new List<MultiFileInfo>(), StringComparer.OrdinalIgnoreCase);
                var unassignedFiles = new List<MultiFileInfo>();

                foreach (var file in masterTorrent.Files)
                {
                    string filename = file.Path.LastOrDefault() ?? "";
                    string matchedRegion = TorrentConstants.DefaultRegions.FirstOrDefault(r => Regex.IsMatch(filename, @"\(" + Regex.Escape(r) + @"(\)|,)", RegexOptions.IgnoreCase));

                    if (matchedRegion != null) regionalBuckets[matchedRegion].Add(file);
                    else unassignedFiles.Add(file);
                }

                if (unassignedFiles.Count > 0)
                {
                    Log($"\n[WARNING] {unassignedFiles.Count} file(s) failed categorization by region.", ConsoleColor.Red);
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
                    if (region.TotalBytes > request.TargetBytes)
                    {
                        if (currentBatchFiles.Count > 0)
                        {
                            SaveSubTorrent(masterTorrent, currentBatchFiles, batchIndex, request.TorrentPath,
                                $"Split-{GetBestBatchSuffix(currentBatchRegions)}",
                                totalOriginalFiles, request.OutputDir);
                            batchIndex++;
                            currentBatchFiles.Clear();
                            currentBatchRegions.Clear();
                            currentBatchBytes = 0;
                        }

                        int partsNeeded = (int)Math.Ceiling((double)region.TotalBytes / request.TargetBytes);
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
                            SaveSubTorrent(masterTorrent, splitBuckets[i], batchIndex, request.TorrentPath,
                                $"Split-{region.Name}-{i + 1}", 
                                totalOriginalFiles, request.OutputDir);
                            batchIndex++;
                        }
                        continue;
                    }

                    if (currentBatchBytes + region.TotalBytes > request.TargetBytes && currentBatchFiles.Count > 0)
                    {
                        SaveSubTorrent(masterTorrent, currentBatchFiles, batchIndex, request.TorrentPath,
                            $"Split-{GetBestBatchSuffix(currentBatchRegions)}", totalOriginalFiles, request.OutputDir);
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
                    SaveSubTorrent(masterTorrent, currentBatchFiles, batchIndex, request.TorrentPath, $"Split-{GetBestBatchSuffix(currentBatchRegions)}",
                        totalOriginalFiles, request.OutputDir);
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

            Log($" - [ ] Generated Batch {index}: {Path.GetFileName(outPath)} " +
                $"({Math.Round((double)batchFiles.Sum(f => f.FileSize) / (1024 * 1024 * 1024), 2)} GB) | " +
                $"Files: {batchFiles.Count}/{totalOriginalFiles}", ConsoleColor.White);
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
                    string suffix = isCurated ? $"Split-Filtered-{batchIndex}" : $"Split-{batchIndex}";
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
                string suffix = isCurated ? $"Split-Filtered-{batchIndex}" : $"Split-{batchIndex}";
                SaveSubTorrent(masterTorrent, currentBatchFiles, batchIndex, originalTorrentPath, suffix, totalOriginalFiles, outputDir);
            }
        }
    }
}