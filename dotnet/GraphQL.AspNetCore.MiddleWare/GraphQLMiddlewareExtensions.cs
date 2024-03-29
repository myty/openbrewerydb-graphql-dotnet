using Microsoft.AspNetCore.Builder;

namespace GraphQL.AspNetCore.Middleware
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
