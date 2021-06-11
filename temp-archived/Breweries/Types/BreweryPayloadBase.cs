using System.Collections.Generic;
using OpenBreweryDB.API.GraphQL.Common;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.API.GraphQL.Breweries
{
    public class BreweryPayloadBase : PayloadBase
    {
        public BreweryPayloadBase(Brewery brewery, string clientMutationId)
            : base(clientMutationId) => Brewery = brewery;

        public BreweryPayloadBase(IReadOnlyList<UserError> errors, string clientMutationId)
            : base(errors, clientMutationId)
        {
        }

        public Brewery Brewery { get; }
    }
}
