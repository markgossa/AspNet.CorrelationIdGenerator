using AspNet.CorrelationIdGenerator;
using Microsoft.AspNetCore.Builder;

namespace AspNet.CorrelationIdGenerator.ApplicationBuilderExtensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder AddCorrelationIdMiddleware(this IApplicationBuilder applicationBuilder)
        => applicationBuilder.UseMiddleware<CorrelationIdMiddleware>();
}
