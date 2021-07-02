using System.Collections.Generic;
using System.Security.Claims;

namespace GraphQL.AspNetCore.Middleware
{
    public class GraphQLUserContext : Dictionary<string, object>
    {
        public ClaimsPrincipal User { get; set; }
    }
}
