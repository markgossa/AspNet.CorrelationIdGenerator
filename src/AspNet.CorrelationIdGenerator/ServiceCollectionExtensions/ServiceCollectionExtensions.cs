using Microsoft.Extensions.DependencyInjection;

namespace AspNet.CorrelationIdGenerator.ServiceCollectionExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCorrelationIdGenerator(this IServiceCollection services)
        {
            services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();

            return services;
        }
    }
}