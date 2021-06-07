using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Relay.Types;
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
                .Resolve(context =>
                {
                    var query = _breweryResolver.ResolveNearbyBreweries(context).ResultObject;
                    var totalCount = query.Count();
                    var (startingIndex, pageSize) = GetStartingIndexAndPageSize(5, totalCount, context);

                    return ConnectionUtils.ToConnection(
                        query.Take(pageSize),
                        context,
                        startingIndex,
                        totalCount);
                });
        }

        private (int startingIndex, int pageSize) GetStartingIndexAndPageSize(int defaultPageSize, int totalCount, IResolveConnectionContext<object> context)
        {
            var (first, last) = GetFirstLast(context);
            var pageSize = last ?? first ?? defaultPageSize;

            if (!string.IsNullOrEmpty(context.After))
            {
                var after = ConnectionUtils.CursorToOffset(context.After);

                if (last.HasValue)
                {
                    var startingIndex = totalCount - last.Value;

                    return (
                        startingIndex: startingIndex < after ? after : startingIndex,
                        pageSize);
                }
            }

            if (!string.IsNullOrEmpty(context.Before))
            {
                var before = ConnectionUtils.CursorToOffset(context.Before);

                if (first.HasValue)
                {
                    var lastIndex = pageSize - 1;

                    return (
                        startingIndex: 0,
                        pageSize: lastIndex > before ? before - 1 : pageSize);
                }

                if (last.HasValue)
                {
                    var startingIndex = before - pageSize - 1;
                    var lastIndex = startingIndex + pageSize - 1;

                    return (
                        startingIndex: startingIndex < 0 ? 0 : startingIndex,
                        pageSize: lastIndex > before ? lastIndex - startingIndex : pageSize);
                }
            }

            return (
                startingIndex: 0,
                pageSize);
        }

        private (int? first, int? last) GetFirstLast(IResolveConnectionContext<object> context)
        {
            if (context.Last == null)
            {
                return (
                    first: context.First,
                    last: context.First.HasValue ? null : context.Last);
            }

            return (
                first: context.First.HasValue ? context.First : null,
                last: context.First.HasValue ? null : context.Last);
        }
    }
}
