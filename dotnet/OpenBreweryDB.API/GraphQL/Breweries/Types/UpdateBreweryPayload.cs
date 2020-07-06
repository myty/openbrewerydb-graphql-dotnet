using HotChocolate;
using HotChocolate.Types;
using OpenBreweryDB.API.GraphQL.Resolvers;
using OpenBreweryDB.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenBreweryDB.API.GraphQL.Breweries
{
    public class UpdateBreweryPayload
    {
        public string ClientMutationId { get; set; }
        public Brewery Brewery { get; set; }
    }
}
