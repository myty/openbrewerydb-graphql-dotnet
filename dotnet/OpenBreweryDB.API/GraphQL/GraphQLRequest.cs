using System.Collections.Generic;
using System.Text.Json.Serialization;
using GraphQL;
using GraphQL.SystemTextJson;

namespace OpenBreweryDB.API.GraphQL
{
    public class GraphQLRequest
    {
        public string Query { get; set; }
        public string OperationName { get; set; }
        public Inputs Variables { get; set; }
    }
}
