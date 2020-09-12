using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;

namespace Tracer
{
    public class TracerMain : ITracer
    {
        private Dictionary<int, ThreadStack> _stacks = new Dictionary<int, ThreadStack>();
        private List<TraceItem> _result = new List<TraceItem>();
        private static object _locker = new object();

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
            AddToTraceResult(watch);
            watch.Start();
        }

        public void StopTrace()
        {
            lock (_locker)
            {
                int CurrentThreadID = Thread.CurrentThread.ManagedThreadId;
                var (watch, item) = _stacks[CurrentThreadID].TraceWatches.Pop();
                watch.Stop();
                item.ElapsedMilliseconds = watch.ElapsedMilliseconds;
            }
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

        private void AddToTraceResult(Stopwatch watch)
        {
            int CurrentThreadID = Thread.CurrentThread.ManagedThreadId;
            int TraceResultThreadIndex;
            lock (_locker)
            {
                TraceResultThreadIndex = GetTraceResultThreadIndex(CurrentThreadID);
                if (TraceResultThreadIndex == -1)
                {
                    _result.Add(new ThreadItem(CurrentThreadID));
                    TraceResultThreadIndex = _result.Count - 1;
                }
            }

            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(2);
            var item = new MethodItem()
            {
                MethodName = sf.GetMethod().Name,
                MethodClassName = sf.GetMethod().DeclaringType.Name,
                ElapsedMilliseconds = 0,
                SubMethods = null,
            };

            lock (_locker)
            {
                if (_stacks.ContainsKey(CurrentThreadID))
                {
                    _stacks[CurrentThreadID].TraceWatches.Push((watch, item));
                }
                else
                {
                    var stack = new ThreadStack();
                    stack.TraceWatches.Push((watch, item));
                    _stacks.Add(CurrentThreadID, stack);
                }

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

    public class ThreadStack
    {
        public Stack<(Stopwatch watch, TraceItem obj)> TraceWatches;

        public ThreadStack()
        {
            TraceWatches = new Stack<(Stopwatch watch, TraceItem obj)>();
        }
    }

}
