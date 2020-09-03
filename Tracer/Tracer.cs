using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracer
{
    public class TracerMain : ITracer
    {
        Stopwatch watch = new Stopwatch();
        public object GetTraceResult()
        {
            return watch.ElapsedMilliseconds;
        }

        public void StartTrace()
        {
            watch.Start();
        }

        public void StopTrace()
        {
            watch.Stop();
        }
    }
}
