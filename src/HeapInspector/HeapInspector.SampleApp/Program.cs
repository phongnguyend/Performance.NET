using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HeapInspector.SampleApp
{
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
                }

                Thread.Sleep(500);

                count++;

                if (count % 100 == 0)
                {
                    GC.Collect();
                }
            }
        }
    }
}
