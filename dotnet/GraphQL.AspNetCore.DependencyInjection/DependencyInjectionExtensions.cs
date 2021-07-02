using Microsoft.Extensions.DependencyInjection;

namespace GraphQL.AspNetCore.DependencyInjection
{
    public interface IGraphQLSchemaConfiguration
    {
        IServiceCollection Configure(IServiceCollection services, bool includeInstrumentation);
    }

    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddGraphQLSchemaConfiguration<T>(
            this IServiceCollection services,
            bool includeInstrumentation = false
        ) where T : IGraphQLSchemaConfiguration, new()
        {
            var graphQLSchemaConfiguration = new T();

            return graphQLSchemaConfiguration.Configure(services, includeInstrumentation);
        }
    }
}
