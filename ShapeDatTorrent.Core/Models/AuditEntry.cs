using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeDatTorrent.Core.Models
{
    public class AuditEntry
    {
        public string Name { get; set; }
        public bool IsKept { get; set; }
        public string Reason { get; set; } // This will hold the <comment> text
    }
}
