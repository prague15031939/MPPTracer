using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracer;

namespace TracerConsole
{
    class Program
    {
        public static ITracer _tracer;
        static void Main(string[] args)
        {
            _tracer = new TracerMain();
            ExampleMethod();
            List<TracerItem> result = _tracer.GetTraceResult();
            DisplayTracerResult(result);
            Console.ReadKey();
        }

        private static void DisplayTracerResult(List<TracerItem> result)
        {
            foreach (TracerItem item in result)
            {
                Console.WriteLine($"{item.MethodClassName} - {item.MethodName} - {item.ElapsedMilliseconds} ms");
                if (item.SubMethods != null)
                    DisplayTracerResult(item.SubMethods);
            }
        }

        public static void ExampleMethod()
        {
            _tracer.StartTrace();
            var obj = new Example();
            obj.DoSmth(_tracer);
            _tracer.StopTrace();
        }
    }

    public class Example
    {
        public void DoSmth(ITracer _tracer)
        {
            _tracer.StartTrace();
            for (int i = 0; i < 100; i++)
                Console.WriteLine("111");
            _tracer.StopTrace();
        }
    }
}
