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
            AnotherExampleMethod();
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
                Console.WriteLine($"{item.MethodClassName} - {item.MethodName} - {item.ElapsedMilliseconds} ms");
                if (item.SubMethods != null)
                    DisplayTraceResult(item.SubMethods, indent + 2);
            }
        }

        public static void ExampleMethod()
        {
            _tracer.StartTrace();
            var obj = new Example();
            obj.DoSmth(_tracer);
            obj.DoSmthMore(_tracer);
            obj.DoSmth(_tracer);
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

        public void DoSmthMore(ITracer _tracer)
        {
            _tracer.StartTrace();
            for (int i = 0; i < 5; i++)
                Console.WriteLine("uuu");
            DoSmth(_tracer);
            _tracer.StopTrace();
        }
    }
}
