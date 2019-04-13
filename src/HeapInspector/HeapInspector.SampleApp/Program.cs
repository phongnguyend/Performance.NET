using Clifton.Tools.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HeapInspector.SampleApp
{
    public struct TestStruct
    {
        public int a;
    }

    public struct TestStruct2
    {
        public int a;
    }

    class Program
    {
        static void Main(string[] args)
        {
            Run();
            Run();
            Console.ReadLine();
        }

        private static void Run()
        {
            int count = 0;

            while (count <= 1000)
            {
                if (true)
                {
                    var f = new FinalizableClass();
                    Task.Factory.StartNew(() => f.Run());
                    Task.Factory.StartNew(() => f.Run2());
                    var pinned = new PinnedObject<TestStruct>();
                    var pinned2 = new PinnedObject<TestStruct2>();
                }

                Thread.Sleep(100);

                count++;

                if (count % 100 == 0)
                {
                    GC.Collect();
                }
            }
        }
    }
}
