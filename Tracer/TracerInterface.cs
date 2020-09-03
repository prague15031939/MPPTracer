using System.Collections.Generic;

namespace Tracer
{
    public interface ITracer
    {
        void StartTrace();
        void StopTrace();
        List<TracerItem> GetTraceResult();
    }
}
