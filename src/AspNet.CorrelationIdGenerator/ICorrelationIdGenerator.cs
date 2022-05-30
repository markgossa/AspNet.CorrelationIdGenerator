namespace AspNet.CorrelationIdGenerator
{
    public interface ICorrelationIdGenerator
    {
        string Get();
        void Set(string correlationId);
    }
}