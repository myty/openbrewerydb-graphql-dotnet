using System.Collections.Generic;
using System.Text.Json.Serialization;
using GraphQL.SystemTextJson;

namespace OpenBreweryDB.API.GraphQL
{
    public class GraphQLRequest
    {
        public string OperationName { get; set; }

        public string Query { get; set; }

        [JsonConverter(typeof(InputsConverter))]
        public Dictionary<string, object> Variables
        {
            get; set;
        }
    }
}
