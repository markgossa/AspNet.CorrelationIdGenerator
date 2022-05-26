using Example.Api;
using Moq;
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
        var response = await _apiTestsContext.HttpClientWithMock.GetAsync(_requestUri);
        
        AssertResponseHeadersContainCorrelationId(response);
        await AssertWeatherForecastsReturned(response);
    }
    
    [Fact]
    public async Task GivenApiIsStarted_WhenICallTheEndpointWithoutACorrelationId_ThenANewOneIsGeneratedAndReturnedInTheResponseHeaderForEachRequest()
    {
        var response1 = await _apiTestsContext.HttpClientWithoutMock.GetAsync(_requestUri);
        var response2 = await _apiTestsContext.HttpClientWithoutMock.GetAsync(_requestUri);

        Assert.NotEqual(GetCorrelationIdFromResponse(response1), GetCorrelationIdFromResponse(response2));
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

        var response = await _apiTestsContext.HttpClientWithMock.SendAsync(request);

        AssertResponseHeadersContainCorrelationId(response, expectedCorrelationId);
        await AssertWeatherForecastsReturned(response);
        _apiTestsContext.MockCorrelationIdGenerator.Verify(m => m.Set(expectedCorrelationId), Times.Once);
    }

    private void AssertResponseHeadersContainCorrelationId(HttpResponseMessage response, 
        string? expectedCorrelationId = null)
    {
        var correlationId = GetCorrelationIdFromResponse(response);
        var expected = string.IsNullOrWhiteSpace(expectedCorrelationId)
            ? _apiTestsContext.CorrelationId
            : expectedCorrelationId;

        Assert.Equal(expected, correlationId);
    }

    private static string? GetCorrelationIdFromResponse(HttpResponseMessage response)
    {
        response.Headers.TryGetValues(_correlationIdHeader, out var correlationIdValues);
     
        return correlationIdValues?.First();
    }

    private static async Task AssertWeatherForecastsReturned(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        var weatherForecast = JsonSerializer.Deserialize<List<WeatherForecast>>(json);
        Assert.NotNull(weatherForecast);
    }
}
