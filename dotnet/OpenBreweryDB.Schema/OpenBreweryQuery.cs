using AndcultureCode.CSharp.Core.Extensions;
using GraphQL;
using GraphQL.Types;
using OpenBreweryDB.Schema.Resolvers;
using OpenBreweryDB.Schema.Types;

namespace OpenBreweryDB.Schema
{
    public class OpenBreweryQuery : ObjectGraphType
    {
        public OpenBreweryQuery(BreweryResolver breweryResolver)
        {
            Name = "Query";

            Connection<BreweryType>()
                .Name("breweries")
                .Resolve(context => breweryResolver
                    .ResolveBreweries(context)
                    .ThrowIfAnyErrors()
                    .GetPagedResults(context));
        }
    }
}
