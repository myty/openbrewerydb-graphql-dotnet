using System.Collections.Generic;
using OpenBreweryDB.API.GraphQL.Common;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.API.GraphQL.Breweries
{
    public class UpdateBreweryPayload : BreweryPayloadBase
    {
        public UpdateBreweryPayload(Brewery brewery, string clientMutationId)
            : base(brewery, clientMutationId)
        {
        }

        public UpdateBreweryPayload(UserError error, string clientMutationId)
            : base(new[] { error }, clientMutationId)
        {
        }

        public UpdateBreweryPayload(IReadOnlyList<UserError> errors, string clientMutationId)
            : base(errors, clientMutationId)
        {
        }
    }
}
