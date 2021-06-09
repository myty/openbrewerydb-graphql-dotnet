using GraphQL.Relay.Types;
using GraphQL.Types;
using OpenBreweryDB.Data.Models;
using OpenBreweryDB.Schema.Resolvers;
using OpenBreweryDB.Schema.Types;

namespace OpenBreweryDB.Schema
{
    public class OpenBreweryQuery : QueryGraphType
    {
        public OpenBreweryQuery()
        {
            Name = "Query";

            BreweryConnection();
            GetBreweryByExternalId();
            NearbyBreweryConnection();
        }

        private void BreweryConnection()
        {
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
                .ScopedResolver<BreweryResolver, object, Brewery>(r => r.ResolveBreweries);
        }

        private void GetBreweryByExternalId()
        {
            Field<BreweryType, Brewery>()
                .Name("breweryByExternalId")
                .Argument<StringGraphType>("external_id", "filter by external id")
                .ScopedResolver<BreweryResolver, object, Brewery>(r => r.ResolveBreweryByExternalId);
        }

        private void NearbyBreweryConnection()
        {
            Connection<BreweryType>()
                .Name("nearbyBreweries")
                .Argument<FloatGraphType>("latitude", "latitude")
                .Argument<FloatGraphType>("longitude", "longitude")
                .Argument<IntGraphType, int>("within", "search radius in miles", 25)
                .ScopedResolver<BreweryResolver, object, Brewery>(r => r.ResolveNearbyBreweries, 5);
        }
    }
}
