using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeapInspector
{
    public class TypeEntry
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public ulong MinSize { get; set; }
        public ulong MaxSize { get; set; }
        public ulong TotalSize { get; set; }
    }
}
