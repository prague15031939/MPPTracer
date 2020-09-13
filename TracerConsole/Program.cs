using System;
using System.Collections.Generic;
using System.Threading;

using Tracer;

namespace TracerConsole
{
    class Program
    {
        public static ITracer _tracer;

        static void Main(string[] args)
        {
            _tracer = new TracerMain();

            var obj = new Example(_tracer);
            Thread AnotherThread = new Thread(new ThreadStart(ramp));
            AnotherThread.Start();
            Thread OneMoreThread = new Thread(new ThreadStart(AnotherExampleMethod));
            OneMoreThread.Start();

            ExampleMethod();
            obj.recursion();
            AnotherExampleMethod();

            AnotherThread.Join();
            OneMoreThread.Join(); 

            var result = _tracer.GetTraceResult();
            DisplayTraceResult(result.root);

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
        private ITracer _tracer;
        private int RecursionCount = 10;

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

        public void recursion()
        {
            if (RecursionCount > 0)
            {
                _tracer.StartTrace();
                RecursionCount--;
                for (int i = 0; i < 1000; i++)
                    Console.WriteLine("111");
                recursion();
                _tracer.StopTrace();
            }
            else
                return;
        }
    }
}