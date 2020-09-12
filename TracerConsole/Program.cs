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
        public static int sho = 10;

        static void Main(string[] args)
        {
            _tracer = new TracerMain();

            Thread AnotherThread = new Thread(new ThreadStart(ramp));
            AnotherThread.Start();
            Thread OneMoreThread = new Thread(new ThreadStart(AnotherExampleMethod));
            OneMoreThread.Start();

            ExampleMethod();
            recursion();
            AnotherExampleMethod();
            while (bar != 0)
                ;
            var result = _tracer.GetTraceResult();
            DisplayTraceResult(result.root);

            ///

            /*ISerializer serializer = new JsonSerializer();
            string JsonLine = serializer.Serialize(result);
            serializer = new CustomXmlSerializer();
            string XmlLine = serializer.Serialize(result);

            IDisplayer displayer = new ConsoleDisplayer();
            displayer.Display(JsonLine);
            displayer.Display(XmlLine);
            displayer = new FileDisplayer(@"D:\\sho.txt");
            displayer.Display(JsonLine);
            displayer = new FileDisplayer(@"D:\\kok.txt");
            displayer.Display(XmlLine);*/

            ///

            Console.ReadKey();
        }

        private static void DisplayTraceResult(List<TraceItem> result, int indent = 0)
        {
            foreach (TraceItem item in result)
            {
                for (int i = 0; i < indent; i++)
                    Console.Write("  ");

                if (item is ThreadItem)
                    Console.WriteLine($"thread #{(item as ThreadItem).ThreadID} - {item.ElapsedMilliseconds} ms:");
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
            for (int i = 0; i < 100; i++)
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

        public static void recursion()
        {
            if (sho > 0)
            {
                _tracer.StartTrace();
                sho--;
                for (int i = 0; i < 1000; i++)
                    Console.WriteLine("111");
                recursion();
                _tracer.StopTrace();
            }
            else
                return;
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
