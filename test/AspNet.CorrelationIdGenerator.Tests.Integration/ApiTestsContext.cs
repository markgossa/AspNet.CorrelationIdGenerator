using Example.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net.Http;

namespace AspNet.CorrelationIdGenerator.Tests.Integration;

public class ApiTestsContext : IDisposable
{
    public HttpClient HttpClient { get; }
    public Mock<ICorrelationIdGenerator> MockCorrelationIdGenerator { get; } = new();
    public string CorrelationId { get; set; }

    public ApiTestsContext()
    {
        HttpClient = BuildWebApplicationFactory().CreateClient();
        CorrelationId = Guid.NewGuid().ToString();
        MockCorrelationIdGenerator.Setup(m => m.Get()).Returns(CorrelationId);
    }

    protected WebApplicationFactory<WeatherForecast> BuildWebApplicationFactory()
        => new WebApplicationFactory<WeatherForecast>()
            .WithWebHostBuilder(b =>
                b.ConfigureServices(services => RegisterServices(services)));

    private void RegisterServices(IServiceCollection services)
        => services.AddSingleton(MockCorrelationIdGenerator.Object);

    public void Dispose()
    {
        HttpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}