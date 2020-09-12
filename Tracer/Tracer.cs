using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;

namespace Tracer
{
    public class TracerMain : ITracer
    {
        private Dictionary<string, Stopwatch> _TraceWatches = new Dictionary<string, Stopwatch>();
        private List<TraceItem> _result = new List<TraceItem>();
        private static object locker = new object();

        public TraceResult GetTraceResult()
        {
            foreach (TraceItem thread in _result)
            {
                thread.ElapsedMilliseconds = 0;
                foreach (TraceItem method in thread.SubMethods)
                {
                    thread.ElapsedMilliseconds += method.ElapsedMilliseconds;
                }
            }
            return new TraceResult(_result);
        }

        public void StartTrace()
        {
            var watch = new Stopwatch();
            string FullMethodName = GetFullMethodName();

            lock (locker)
            {
                if (_TraceWatches.ContainsKey(FullMethodName))
                    _TraceWatches[FullMethodName] = watch;
                else
                    _TraceWatches.Add(GetFullMethodName(), watch);
            }
            AddToTraceResult();
            watch.Start();
        }

        public void StopTrace()
        {
            string FullMethodName = GetFullMethodName();
            _TraceWatches[FullMethodName].Stop();
            string MethodName = FullMethodName.Substring(FullMethodName.LastIndexOf('.') + 1);

            int CurrentThreadID = Thread.CurrentThread.ManagedThreadId;
            lock (locker)
            {
                int TraceResultThreadIndex = GetTraceResultThreadIndex(CurrentThreadID);

                ModifyTraceItemElapsedTime((_result[TraceResultThreadIndex] as ThreadItem).SubMethods, MethodName, _TraceWatches[FullMethodName].ElapsedMilliseconds);
            }
        }

        private string GetFullMethodName()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(2);
            int CurrentThreadID = Thread.CurrentThread.ManagedThreadId;
            return $"{CurrentThreadID}:{sf.GetMethod().DeclaringType.FullName}.{sf.GetMethod().Name}";
        }

        private int GetTraceResultThreadIndex(int TargetID)
        {
            foreach (TraceItem item in _result)
            {
                if ((item as ThreadItem).ThreadID == TargetID)
                    return _result.IndexOf(item);
            }
            return -1;
        }

        private void ModifyTraceItemElapsedTime(List<TraceItem> ResultNode, string target, long time)
        {
            TraceItem LastElement = ResultNode[ResultNode.Count - 1];
            if ((LastElement as MethodItem).MethodName == target)
                LastElement.ElapsedMilliseconds = time;
            else if (LastElement.SubMethods != null)
                ModifyTraceItemElapsedTime(LastElement.SubMethods, target, time);
        }

        private void AddToTraceResult()
        {
            int CurrentThreadID = Thread.CurrentThread.ManagedThreadId;
            int TraceResultThreadIndex;
            lock (locker)
            {
                TraceResultThreadIndex = GetTraceResultThreadIndex(CurrentThreadID);
                if (TraceResultThreadIndex == -1)
                { // is absent
                    _result.Add(new ThreadItem(CurrentThreadID));
                    TraceResultThreadIndex = _result.Count - 1;
                    //TraceResultThreadIndex = InsertThreadIntoTraceResult(CurrentThreadID);
                }
            }

            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(2);
            string FullMethodName = $"{CurrentThreadID}:{sf.GetMethod().DeclaringType.FullName}.{sf.GetMethod().Name}";
            var item = new MethodItem()
            {
                MethodName = sf.GetMethod().Name,
                MethodClassName = sf.GetMethod().DeclaringType.Name,
                ElapsedMilliseconds = _TraceWatches[FullMethodName].ElapsedMilliseconds,
                SubMethods = null,
            };

            lock (locker)
            {
                InsertIntoTree((_result[TraceResultThreadIndex] as ThreadItem).SubMethods, item, st.GetFrames());
            }
        }

        private void InsertIntoTree(List<TraceItem> ResultNode, MethodItem item, StackFrame[] sfList)
        {
            try
            {
                for (int i = 3; i < sfList.Length; i++)
                {
                    TraceItem LastElement = ResultNode[ResultNode.Count - 1];
                    if (sfList[i].GetMethod().Name == (LastElement as MethodItem).MethodName)
                    {
                        if (LastElement.SubMethods != null)
                            InsertIntoTree(LastElement.SubMethods, item, sfList);
                        else
                        {
                            var SubList = new List<TraceItem>();
                            SubList.Add(item);
                            LastElement.SubMethods = SubList;
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

}
