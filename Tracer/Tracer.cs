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
        private Dictionary<string, Stopwatch> _TraceWatches = new Dictionary<string, Stopwatch>();
        private List<TraceItem> _result = new List<TraceItem>();

        public List<TraceItem> GetTraceResult()
        {
            return _result;
        }

        public void StartTrace()
        {
            var watch = new Stopwatch();
            string FullMethodName = GetFullMethodName();
            if (_TraceWatches.ContainsKey(FullMethodName))
                _TraceWatches[FullMethodName] = watch;
            else
                _TraceWatches.Add(GetFullMethodName(), watch);
            AddToTraceResult();
            watch.Start();
        }

        public void StopTrace()
        {
            string FullMethodName = GetFullMethodName();
            _TraceWatches[FullMethodName].Stop();
            string MethodName = FullMethodName.Substring(FullMethodName.LastIndexOf('.') + 1);
            ModifyTraceItemElapsedTime(_result, MethodName, _TraceWatches[FullMethodName].ElapsedMilliseconds);
        }

        private string GetFullMethodName()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(2);
            return $"{sf.GetMethod().DeclaringType.FullName}.{sf.GetMethod().Name}";
        }

        private void ModifyTraceItemElapsedTime(List<TraceItem> ResultNode, string target, long time)
        {
            var LastElement = ResultNode[ResultNode.Count - 1];
            if (LastElement.MethodName == target)
            {
                LastElement.ElapsedMilliseconds = time;
            }
            else if (LastElement.SubMethods != null)
                ModifyTraceItemElapsedTime(LastElement.SubMethods, target, time);
        }

        private void AddToTraceResult()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(2);
            string FullMethodName = $"{sf.GetMethod().DeclaringType.FullName}.{sf.GetMethod().Name}";
            var item = new TraceItem()
            {
                MethodName = sf.GetMethod().Name,
                MethodClassName = sf.GetMethod().DeclaringType.Name,
                ElapsedMilliseconds = _TraceWatches[FullMethodName].ElapsedMilliseconds,
                SubMethods = null,
            };

            InsertIntoTree(_result, item, st.GetFrames());
        }

        private void InsertIntoTree(List<TraceItem> ResultNode, TraceItem item, StackFrame[] sfList)
        {
            try
            {
                for (int i = 3; i < sfList.Length; i++)
                {
                    if (sfList[i].GetMethod().Name == ResultNode[ResultNode.Count - 1].MethodName)
                    {
                        if (ResultNode[ResultNode.Count - 1].SubMethods != null)
                            InsertIntoTree(ResultNode[ResultNode.Count - 1].SubMethods, item, sfList);
                        else
                        {
                            var SubList = new List<TraceItem>();
                            SubList.Add(item);
                            ResultNode[ResultNode.Count - 1].SubMethods = SubList;
                        }
                        return;
                    }
                }
                ResultNode.Add(item);
            }
            catch
            {
                ResultNode.Add(item);
            }
        }

    }

    public class TraceItem
    {
        public long ElapsedMilliseconds { get; set; }
        public string MethodName { get; set; }
        public string MethodClassName { get; set; }
        public List<TraceItem> SubMethods { get; set; }
    }
}
