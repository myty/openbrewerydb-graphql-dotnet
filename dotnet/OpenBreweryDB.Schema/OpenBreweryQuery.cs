using System.Collections.Generic;
using GraphQL.Types;
using OpenBreweryDB.Data.Models;
using OpenBreweryDB.Schema.Resolvers;
using OpenBreweryDB.Schema.Types;

namespace OpenBreweryDB.Schema
{
    public class OpenBreweryQuery : ObjectGraphType
    {
        public OpenBreweryQuery(BreweryResolver breweryResolver)
        {
            Name = "Query";

            Field<NonNullGraphType<ListGraphType<BreweryType>>, IEnumerable<Brewery>>()
                .Name("breweries")
                .Argument<IntGraphType>("take")
                .Resolve(breweryResolver.ResolveBreweries);
        }
    }
}
