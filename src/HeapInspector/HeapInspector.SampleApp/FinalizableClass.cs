using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HeapInspector.SampleApp
{
    public class FinalizableClass
    {
        object _lock = new object();
        ~FinalizableClass()
        {
            Thread.Sleep(1000);
        }

        public void Run()
        {
            lock(_lock)
            {
                Thread.Sleep(2000);
            }
        }

    }
}
