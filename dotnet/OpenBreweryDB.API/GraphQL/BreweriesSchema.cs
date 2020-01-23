using GraphQL.Types;
using GraphQL.Utilities;
using OpenBreweryDB.API.GraphQL.Mutations;
using OpenBreweryDB.API.GraphQL.Queries;
using System;

namespace OpenBreweryDB.API.GraphQL
{
    public class BreweriesSchema : Schema
    {
        public BreweriesSchema(IServiceProvider provider)
            : base(provider)
        {
            Query = provider.GetRequiredService<BreweriesQuery>();
            Mutation = provider.GetRequiredService<BreweriesMutation>();
        }
    }
}