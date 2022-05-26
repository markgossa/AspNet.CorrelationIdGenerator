namespace AspNet.CorrelationIdGenerator;

public interface ICorrelationIdGenerator
{
    public string CorrelationId { get; set; }
}
