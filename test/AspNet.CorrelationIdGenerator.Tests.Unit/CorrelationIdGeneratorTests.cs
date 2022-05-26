using Xunit;

namespace AspNet.CorrelationIdGenerator.Tests.Unit;
public class CorrelationIdGeneratorTests
{
    [Fact]
    public void GivenNewInstance_WhenICallGet_ReturnsGuidAsString()
    {
        var sut = new CorrelationIdGenerator();
        var correlationId = sut.Get();

        Assert.True(Guid.TryParse(correlationId, out _));
    }
    
    [Fact]
    public void GivenNewInstance_WhenICallSet_SavesCorrelationId()
    {
        var savedCorrelationId = Guid.NewGuid().ToString();
        var sut = new CorrelationIdGenerator();
        sut.Set(savedCorrelationId);

        Assert.Equal(savedCorrelationId, sut.Get());
    }
}