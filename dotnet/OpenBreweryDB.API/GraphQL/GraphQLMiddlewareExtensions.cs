using System;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Instrumentation;
using GraphQL.SystemTextJson;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace OpenBreweryDB.API.GraphQL
{
    public static class GraphQLMiddlewareExtensions
    {
        public static IApplicationBuilder UseGraphQL(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GraphQLMiddleware>()
                .UseGraphQLAltair(path: "/api/graphql");
        }
    }
}
