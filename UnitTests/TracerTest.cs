using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Tracer;

namespace UnitTests
{
    [TestClass]
    public class TracerTest
    {
        private TracerMain _tracer;
        private TraceResult _result;

        [TestInitialize]
        public void Setup()
        {
            _tracer = new TracerMain();

            var obj1 = new Example1(_tracer);
            var obj2 = new Example2(_tracer);
            var obj3 = new Example1(_tracer);

            Thread AnotherThread = new Thread(new ThreadStart(obj1.ExampleMethod));
            AnotherThread.Start();
            Thread.Sleep(1000);
            Thread OneMoreThread = new Thread(new ThreadStart(obj2.DoSmthMore));
            OneMoreThread.Start();
            Thread.Sleep(1000);

            obj3.ExampleMethod();
            obj3.recursion();
            obj3.AnotherExampleMethod();

            AnotherThread.Join();
            OneMoreThread.Join();
            _result = _tracer.GetTraceResult();
        }

        [TestMethod]
        public void TestThreadCount()
        {
            Assert.AreEqual(3, _result.root.Count);
        }

        [TestMethod]
        public void TestMethodNesting()
        {
            Assert.AreEqual(3, _result.root[0].SubMethods[0].SubMethods.Count);
        }

        [TestMethod]
        public void TestMethodName()
        {
            var ExampleMethod = _result.root[0].SubMethods[0];
            Assert.AreEqual((ExampleMethod.SubMethods[0] as MethodItem).MethodName, ((ExampleMethod.SubMethods[1] as MethodItem).SubMethods[0] as MethodItem).MethodName);
            Assert.AreEqual("Example1", (_result.root[2].SubMethods[2] as MethodItem).MethodClassName);
        }

        [TestMethod]
        public void TestExecutionTime()
        {
            Assert.IsTrue(_result.root[2].ElapsedMilliseconds > _result.root[0].ElapsedMilliseconds);
            Assert.IsTrue(_result.root[2].SubMethods[1].ElapsedMilliseconds > 4000);
        }
    }

    public class Example1
    {
        private ITracer _tracer;
        private int RecursionCount = 10;

        public Example1(ITracer tracer)
        {
            _tracer = tracer;
        }

        public void ExampleMethod()
        {
            _tracer.StartTrace();
            var obj = new Example2(_tracer);
            obj.DoSmth();
            obj.DoSmthMore();
            obj.DoSmth();
            _tracer.StopTrace();
        }

        public void AnotherExampleMethod()
        {
            _tracer.StartTrace();
            Thread.Sleep(2000);
            _tracer.StopTrace();
        }

        public void recursion()
        {
            if (RecursionCount > 0)
            {
                _tracer.StartTrace();
                RecursionCount--;
                Thread.Sleep(500);
                recursion();
                _tracer.StopTrace();
            }
            else
                return;
        }
    }

    public class Example2
    {
        public ITracer _tracer;

        public Example2(ITracer tracer)
        {
            _tracer = tracer;
        }

        public void DoSmth()
        {
            _tracer.StartTrace();
            Thread.Sleep(1000);
            _tracer.StopTrace();
        }

        public void DoSmthMore()
        {
            _tracer.StartTrace();
            Thread.Sleep(1000);
            DoSmth();
            _tracer.StopTrace();
        }
    }
}

