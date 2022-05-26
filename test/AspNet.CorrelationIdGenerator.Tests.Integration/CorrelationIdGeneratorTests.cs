using Example.Api;
using System.Net.Http;
using System.Text.Json;
using Xunit;

namespace AspNet.CorrelationIdGenerator.Tests.Integration;
public class CorrelationIdGeneratorTests : IClassFixture<ApiTestsContext>
{
    private const string _correlationIdHeader = "X-Correlation-Id";
    private const string _requestUri = "WeatherForecast";
    private readonly ApiTestsContext _apiTestsContext;

    public CorrelationIdGeneratorTests(ApiTestsContext apiTestsContext) 
        => _apiTestsContext = apiTestsContext;

    [Fact]
    public async Task GivenApiIsStarted_WhenICallTheEndpointWithoutACorrelationId_ThenOneIsGeneratedAndReturnedInTheResponseHeader()
    {
        var response = await _apiTestsContext.HttpClient.GetAsync(_requestUri);
        
        AssertResponseHeadersContainCorrelationId(response);
        await AssertWeatherForecastsReturned(response);
    }
    
    [Fact]
    public async Task GivenApiIsStarted_WhenICallTheEndpointWithACorrelationId_ThenItIsSavedAndReturnedInTheResponseHeader()
    {
        var expectedCorrelationId = Guid.NewGuid().ToString();
        var request = new HttpRequestMessage()
        {
            RequestUri = new Uri(_requestUri, UriKind.Relative)
        };

        request.Headers.Add(_correlationIdHeader, expectedCorrelationId);

        var response = await _apiTestsContext.HttpClient.SendAsync(request);

        AssertResponseHeadersContainCorrelationId(response, expectedCorrelationId);
        await AssertWeatherForecastsReturned(response);
        Assert.Equal(expectedCorrelationId, _apiTestsContext.CorrelationId);
    }

    private void AssertResponseHeadersContainCorrelationId(HttpResponseMessage response, 
        string? expectedCorrelationId = null)
    {
        response.Headers.TryGetValues(_correlationIdHeader, out var correlationIdValues);
        var expected = string.IsNullOrWhiteSpace(expectedCorrelationId) 
            ? _apiTestsContext.CorrelationId 
            : expectedCorrelationId;

        Assert.Equal(expected, correlationIdValues?.First());
    }

    private static async Task AssertWeatherForecastsReturned(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        var weatherForecast = JsonSerializer.Deserialize<List<WeatherForecast>>(json);
        Assert.NotNull(weatherForecast);
    }

    //Assert existing CorrelationId from request header is saved as the CorrelationId in the CorrelationIdGenerator
}
