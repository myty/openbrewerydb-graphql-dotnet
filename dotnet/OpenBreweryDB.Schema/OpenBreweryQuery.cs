using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using OpenBreweryDB.Data.Models;
using OpenBreweryDB.Schema.Resolvers;
using OpenBreweryDB.Schema.Types;

namespace OpenBreweryDB.Schema
{
    public class OpenBreweryQuery : ObjectGraphType
    {
        private readonly BreweryResolver _breweryResolver;

        public OpenBreweryQuery(BreweryResolver breweryResolver)
        {
            _breweryResolver = breweryResolver;

            Name = "Query";

            Field<NodeInterface>()
                .Name("node")
                .Description("Fetches an object given its global Id")
                .Argument<NonNullGraphType<IdGraphType>>("id", "The global Id of the object")
                .ResolveByGlobalId();

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
                .Resolve(context => _breweryResolver
                    .ResolveBreweries(context)
                    .ToOffsetConnection(context));
        }

        private void GetBreweryByExternalId()
        {
            Field<BreweryType, Brewery>()
                .Name("breweryByExternalId")
                .Argument<StringGraphType>("external_id", "filter by external id")
                .Resolve(context => _breweryResolver
                    .ResolveBreweryByExternalId(context)
                    .ResultObject);
        }

        private void NearbyBreweryConnection()
        {
            Connection<BreweryType>()
                .Name("nearbyBreweries")
                .Argument<FloatGraphType>("latitude", "latitude")
                .Argument<FloatGraphType>("longitude", "longitude")
                .Argument<IntGraphType, int>("within", "search radius in miles", 25)
                .Resolve(context => _breweryResolver
                    .ResolveNearbyBreweries(context)
                    .ToOffsetConnection(context));
        }
    }
}
