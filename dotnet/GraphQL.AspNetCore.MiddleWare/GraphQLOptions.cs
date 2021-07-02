using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace GraphQL.AspNetCore.Middleware
{
    public class GraphQLOptions
    {
        public PathString Path { get; set; } = "/api/graphql";

        public Func<HttpContext, IDictionary<string, object>> BuildUserContext { get; set; }

        public bool EnableMetrics { get; set; }
    }
}
