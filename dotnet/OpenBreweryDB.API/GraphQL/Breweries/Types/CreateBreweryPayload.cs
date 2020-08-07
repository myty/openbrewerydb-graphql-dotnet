using System.Collections.Generic;
using OpenBreweryDB.API.GraphQL.Common;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.API.GraphQL.Breweries
{
    public class CreateBreweryPayload : BreweryPayloadBase
    {
        public CreateBreweryPayload(Brewery brewery, string clientMutationId)
            : base(brewery, clientMutationId)
        {
        }

        public CreateBreweryPayload(IReadOnlyList<UserError> errors, string clientMutationId)
            : base(errors, clientMutationId)
        {
        }
    }
}
