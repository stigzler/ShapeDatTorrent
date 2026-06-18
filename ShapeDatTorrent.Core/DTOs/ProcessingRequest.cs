using ShapeDatTorrent.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeDatTorrent.Core.DTOs
{
    public class ProcessingRequest
    {
        public string TorrentPath { get; set; }
        public string KeptDatPath { get; set; }
        public string RemovedDatPath { get; set; }
        public long TargetBytes { get; set; }
        public List<string> Regions { get; set; } = TorrentConstants.DefaultRegions;
        public string OutputDir { get; set; }
        public bool Strict { get; set; }
    }
}
