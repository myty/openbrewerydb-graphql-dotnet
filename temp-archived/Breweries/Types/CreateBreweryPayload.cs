using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using OpenBreweryDB.API.GraphQL.Common;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.API.GraphQL.Breweries
{
    public class CreateBreweryPayload
    {
        public string BreweryId { get; set; }
        public string Name { get; set; }
        public string BreweryType { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string WebsiteURL { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public IEnumerable<string> Tags { get; set; }
    }

    public class CreateBreweryPayloadType : MutationPayloadGraphType<CreateBreweryPayload>
    {
        public CreateBreweryPayloadType()
        {
            Name = nameof(CreateBreweryPayload);

            Interface<ClientMutationBaseType>();
        }

        public override CreateBreweryPayload MutateAndGetPayload(
            MutationInputs inputs,
            ResolveFieldContext<CreateBreweryPayload> context
        )
        {
            var todo = Database.AddTodo(inputs.Get<string>("text"));

            return new
            {
                TodoEdge = new Edge<Todo>
                {
                    Node = todo,
                    Cursor = ConnectionUtils.CursorForObjectInConnection(Database.GetTodos(), todo)
                },
                Viewer = Database.GetViewer(),
            };
        }
    }
}
