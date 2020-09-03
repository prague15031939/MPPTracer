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
        private Dictionary<string, Stopwatch> TracerWatches = new Dictionary<string, Stopwatch>();
        private List<TracerItem> result = new List<TracerItem>();

        public List<TracerItem> GetTraceResult()
        {
            return result;
        }

        public void StartTrace()
        {
            var watch = new Stopwatch();
            TracerWatches.Add(GetFullMethodName(), watch);
            watch.Start();
        }

        public void StopTrace()
        {
            TracerWatches[GetFullMethodName()].Stop();
            AddToTraceResult();
        }

        private string GetFullMethodName()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(2);
            return $"{sf.GetMethod().DeclaringType.FullName}.{sf.GetMethod().Name}";
        }

        private void AddToTraceResult()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(2);
            string FullMethodName = $"{sf.GetMethod().DeclaringType.FullName}.{sf.GetMethod().Name}";
            var item = new TracerItem()
            {
                MethodName = sf.GetMethod().Name,
                MethodClassName = sf.GetMethod().DeclaringType.Name,
                ElapsedMilliseconds = TracerWatches[FullMethodName].ElapsedMilliseconds,
                SubMethods = null,
            };
            result.Add(item);
        }
    }

    public class TracerItem
    {
        public long ElapsedMilliseconds { get; set; }
        public string MethodName { get; set; }
        public string MethodClassName { get; set; }
        public List<TracerItem> SubMethods { get; set; }
    }
}
