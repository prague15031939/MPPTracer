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
        private Stopwatch watch = new Stopwatch();
        private TracerResult result = new TracerResult();

        public TracerResult GetTraceResult()
        {
            return result;
        }

        public void StartTrace()
        {
            watch.Start();
        }

        public void StopTrace()
        {
            watch.Stop();
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            result.CurrentMethodName = sf.GetMethod().Name;
            result.CurrentMethodClassName = sf.GetMethod().DeclaringType.Name;
            result.ElapsedMilliseconds = watch.ElapsedMilliseconds;
        }
    }

    public class TracerResult
    {
        public long ElapsedMilliseconds { get; set; }
        public string CurrentMethodName { get; set; }
        public string CurrentMethodClassName { get; set; }
    }
}
