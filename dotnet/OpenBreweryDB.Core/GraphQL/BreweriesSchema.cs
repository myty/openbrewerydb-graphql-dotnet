using GraphQL.Types;
using GraphQL.Utilities;
using System;

namespace OpenBreweryDB.Core.GraphQL
{
    public class BreweriesSchema : Schema
    {
        public BreweriesSchema(IServiceProvider provider)
            : base(provider)
        {
            Query = provider.GetRequiredService<BreweriesQuery>();
            // Mutation = provider.GetRequiredService<BreweriesMutation>();
        }
    }
}