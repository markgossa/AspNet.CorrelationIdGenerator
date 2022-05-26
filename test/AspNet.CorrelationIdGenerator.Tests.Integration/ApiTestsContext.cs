using Example.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net.Http;

namespace AspNet.CorrelationIdGenerator.Tests.Integration;

public class ApiTestsContext : IDisposable
{
    public HttpClient HttpClientWithMock { get; }
    public HttpClient HttpClientWithoutMock { get; }
    public Mock<ICorrelationIdGenerator> MockCorrelationIdGenerator { get; } = new();
    public string CorrelationId { get; set; }

    public ApiTestsContext()
    {
        HttpClientWithMock = BuildWebApplicationFactoryWithMock().CreateClient();
        HttpClientWithoutMock = BuildWebApplicationFactoryWithoutMock().CreateClient();
        CorrelationId = Guid.NewGuid().ToString();
        MockCorrelationIdGenerator.Setup(m => m.Get()).Returns(CorrelationId);
    }

    protected WebApplicationFactory<WeatherForecast> BuildWebApplicationFactoryWithMock()
        => new WebApplicationFactory<WeatherForecast>()
            .WithWebHostBuilder(b =>
                b.ConfigureServices(services => services.AddScoped(sp => MockCorrelationIdGenerator.Object)));

    protected static WebApplicationFactory<WeatherForecast> BuildWebApplicationFactoryWithoutMock() => new();

    public void Dispose()
    {
        HttpClientWithMock.Dispose();
        HttpClientWithoutMock.Dispose();
        GC.SuppressFinalize(this);
    }
}