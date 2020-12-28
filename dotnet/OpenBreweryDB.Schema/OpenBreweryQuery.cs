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
                .Argument<StringGraphType>("brewery_id", "filter by brewery id")
                .Argument<StringGraphType>("state", "filter by state")
                .Argument<StringGraphType>("type", "filter by type")
                .Argument<StringGraphType>("city", "filter by city name")
                .Argument<StringGraphType>("name", "search by brewery name")
                .Argument<StringGraphType>("search", "general search")
                .Argument<ListGraphType<StringGraphType>>("sort", "sort by")
                .Argument<ListGraphType<StringGraphType>>("tags", "filter by tags")
                .Resolve(context => breweryResolver
                    .ResolveBreweries(context)
                    .ToConnection(context));

            Connection<BreweryType>()
                .Name("nearbyBreweries")
                .Argument<DecimalGraphType>("latitude", "latitude")
                .Argument<DecimalGraphType>("longitude", "longitude")
                .Argument<IntGraphType, int>("within", "search radius in miles", 25)
                .Resolve(context => breweryResolver
                    .ResolveNearbyBreweries(context)
                    .ToConnection(context));
        }
    }
}
