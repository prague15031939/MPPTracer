using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Tracer
{
    [Serializable]
    public class TraceItem
    {
        [XmlAttribute("time")]
        [JsonProperty("time")]
        public long ElapsedMilliseconds { get; set; }

        [XmlElement("method")]
        [JsonProperty("methods")]
        public List<TraceItem> SubMethods { get; set; }
    }

    [Serializable]
    public class MethodItem : TraceItem
    {
        [XmlAttribute("name")]
        [JsonProperty("name")]
        public string MethodName { get; set; }

        [XmlAttribute("class")]
        [JsonProperty("class")]
        public string MethodClassName { get; set; }
    }

    [Serializable]
    public class ThreadItem : TraceItem
    {
        [XmlAttribute("id")]
        [JsonProperty("id")]
        public int ThreadID { get; set; }

        public ThreadItem() { }

        public ThreadItem(int id)
        {
            ThreadID = id;
            SubMethods = new List<TraceItem>();
        }
    }

    [Serializable]
    [XmlInclude(typeof(ThreadItem)), XmlInclude(typeof(MethodItem))]
    [XmlRoot("root")]
    public class TraceResult
    {
        [XmlElement("thread")]
        [JsonProperty("threads")]
        public List<TraceItem> root;

        public TraceResult() { }

        public TraceResult(List<TraceItem> result)
        {
            root = result;
        }
    }
}
