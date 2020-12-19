using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace OpenBreweryDB.API.GraphQL
{
    public class GraphQLOptions
    {
        public PathString Path { get; set; } = "/api/graphql";

        public Func<HttpContext, IDictionary<string, object>> BuildUserContext { get; set; }

        public bool EnableMetrics { get; set; }
    }
}
