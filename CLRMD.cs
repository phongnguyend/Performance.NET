using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LargeObjectHeap
{
    public class TypeEntry
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public ulong MinSize { get; set; }
        public ulong MaxSize { get; set; }
        public ulong TotalSize { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string pid = "5372";
            using (DataTarget target = DataTarget.AttachToProcess(Int32.Parse(pid), 10000))
            {
                ClrRuntime runtime = target.ClrVersions.First().CreateRuntime();

                Dictionary<string, TypeEntry> types = new Dictionary<string, TypeEntry>();
                ClrHeap heap = runtime.Heap;
                foreach (ulong obj in heap.EnumerateObjects())
                {
                    ClrType type = heap.GetObjectType(obj);
                    var size = type.GetSize(obj);
                    if (types.ContainsKey(type.Name))
                    {
                        types[type.Name].Count++;
                        types[type.Name].MinSize = Math.Min(types[type.Name].MinSize, size);
                        types[type.Name].MaxSize = Math.Max(types[type.Name].MaxSize, size);
                        types[type.Name].TotalSize += size;
                    }
                    else
                    {
                        types[type.Name] = new TypeEntry
                        {
                            Name = type.Name,
                            Count = 1,
                            MinSize = size,
                            MaxSize = size,
                            TotalSize = size
                        };
                    }

                    if (type.GetSize(obj) >= 85000)
                    {

                    }
                }

                foreach (var val in types)
                {
                    var infor = val.Value;
                    Console.WriteLine($"{infor.Name}: {infor.Count} | {infor.MinSize} | {infor.MaxSize} | {infor.TotalSize}");
                }
            }

            Console.ReadLine();
        }
    }
}
