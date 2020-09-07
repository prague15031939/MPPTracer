using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracer;

namespace TracerConsole
{
    class Program
    {
        public static ITracer _tracer;
        public static int bar = 3;
        static void Main(string[] args)
        {
            _tracer = new TracerMain();

            Thread AnotherThread = new Thread(new ThreadStart(ramp));
            AnotherThread.Start();
            Thread OneMoreThread = new Thread(new ThreadStart(AnotherExampleMethod));
            OneMoreThread.Start();

            ExampleMethod();
            AnotherExampleMethod();
            while (bar != 0)
                ;
            List<TraceItem> result = _tracer.GetTraceResult();
            DisplayTraceResult(result);
            Console.ReadKey();
        }

        private static void DisplayTraceResult(List<TraceItem> result, int indent = 0)
        {
            foreach (TraceItem item in result)
            {
                for (int i = 0; i < indent; i++)
                    Console.Write("  ");

                if (item is ThreadItem)
                    Console.WriteLine($"thread #{(item as ThreadItem).ThreadID}:");
                else if (item is MethodItem)
                {
                    MethodItem mItem = item as MethodItem;
                    Console.WriteLine($"{mItem.MethodClassName} - {mItem.MethodName} - {mItem.ElapsedMilliseconds} ms");
                }
                if (item.SubMethods != null)
                    DisplayTraceResult(item.SubMethods, indent + 2);
            }
        }

        public static void ExampleMethod()
        {
            _tracer.StartTrace();
            var obj = new Example(_tracer);
            obj.DoSmth();
            obj.DoSmthMore();
            obj.DoSmth();
            _tracer.StopTrace();
        }

        public static void AnotherExampleMethod()
        {
            _tracer.StartTrace();
            /*var obj = new Example();
            obj.DoSmth(_tracer);
            obj.DoSmth(_tracer);*/
            for (int i = 0; i < 10; i++)
                Console.WriteLine("kkk");
            _tracer.StopTrace();
            bar--;
        }

        public static void ramp()
        {
            _tracer.StartTrace();
            AnotherExampleMethod();
            _tracer.StopTrace();
        }
    }

    public class Example
    {
        public static ITracer _tracer;

        public Example(ITracer tracer)
        {
            _tracer = tracer;
        }

        public void DoSmth()
        {
            _tracer.StartTrace();
            for (int i = 0; i < 100; i++)
                Console.WriteLine("111");
            _tracer.StopTrace();
        }

        public void DoSmthMore()
        {
            _tracer.StartTrace();
            for (int i = 0; i < 5; i++)
                Console.WriteLine("uuu");
            DoSmth();
            _tracer.StopTrace();
        }
    }
}
