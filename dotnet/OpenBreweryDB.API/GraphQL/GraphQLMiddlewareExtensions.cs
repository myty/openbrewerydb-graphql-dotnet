using System;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Instrumentation;
using GraphQL.Server.Ui.Altair;
using GraphQL.SystemTextJson;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace OpenBreweryDB.API.GraphQL
{
    public static class GraphQLMiddlewareExtensions
    {
        public static IApplicationBuilder UseGraphQL(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<GraphQLMiddleware>();

            builder.UseGraphQLAltair(new GraphQLAltairOptions
            {
                GraphQLEndPoint = "/api/graphql"
            });

            return builder;
        }

        public static IServiceCollection AddGraphQL(this IServiceCollection services, Action<GraphQLOptions> action = null)
        {
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IDocumentWriter, DocumentWriter>();
            services.AddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>();
            services.AddSingleton<DataLoaderDocumentListener>();
            services.AddSingleton<InstrumentFieldsMiddleware>();

            if (action != null)
            {
                return services.Configure(action);
            }

            return services;
        }
    }
}
