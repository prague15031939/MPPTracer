namespace Tracer
{
    public interface ITracer
    {
        void StartTrace();
        void StopTrace();
        TracerResult GetTraceResult();
    }
}
