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
            var obj = new Example();
            var _tracer2 = new TracerMain();
            _tracer.StartTrace();
            obj.DoSmth(_tracer2);
            _tracer.StopTrace();
            TracerResult result = _tracer.GetTraceResult();
            Console.WriteLine($"{result.CurrentMethodClassName} - {result.CurrentMethodName} - {result.ElapsedMilliseconds} ms");
            Console.ReadKey();
        }
    }

    public class Example
    {
        public void DoSmth(ITracer _tracer)
        {
            _tracer.StartTrace();
            for (int i = 0; i < 100; i++)
                Console.WriteLine("sho");
            _tracer.StopTrace();
            TracerResult result = _tracer.GetTraceResult();
            Console.WriteLine($"{result.CurrentMethodClassName} - {result.CurrentMethodName} - {result.ElapsedMilliseconds} ms");
        }
    }
}
