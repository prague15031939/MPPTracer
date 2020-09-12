using System;
using System.IO;
using Tracer;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace TracerConsole
{
    public interface ISerializer
    {
        string Serialize(TraceResult obj);
    }

    public class JsonSerializer : ISerializer
    {
        private JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
            NullValueHandling = NullValueHandling.Include,
            Formatting = Newtonsoft.Json.Formatting.Indented,
        };

        public string Serialize(TraceResult obj)
        {
            return JsonConvert.SerializeObject(obj, JsonSettings);
        }
    }

    public class CustomXmlSerializer : ISerializer
    {
        public string Serialize(TraceResult obj)
        {
            MemoryStream SerializationStream = new MemoryStream();
            XmlSerializer formatter = new XmlSerializer(typeof(TraceResult));
            formatter.Serialize(SerializationStream, obj);
            return System.Text.Encoding.Default.GetString(SerializationStream.ToArray());
        }
    }
}
