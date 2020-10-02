using AndcultureCode.CSharp.Core.Extensions;
using AutoMapper;
using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types.Relay;
using OpenBreweryDB.Core.Conductors;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Core.Extensions;
using OpenBreweryDB.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace OpenBreweryDB.API.GraphQL.Breweries
{
    public partial class BreweryQueries
    {
        [UsePaging]
        public IQueryable<Brewery> GetBreweries(
            [GraphQLDescription("filter by brewery id"), GraphQLName("brewery_id")] string breweryId,
            [GraphQLDescription("filter by state")] string state,
            [GraphQLDescription("filter by type")] string type,
            [GraphQLDescription("search by city name")] string city,
            [GraphQLDescription("search by brewery name")] string name,
            [GraphQLDescription("general search")] string search,
            [GraphQLDescription("sort by")] List<string> sort,
            [GraphQLDescription("filter by tags")] List<string> tags,
            IResolverContext ctx,
            [Service] IBreweryConductor breweryConductor,
            [Service] IBreweryValidationConductor validationConductor,
            [Service] IBreweryFilterConductor filterConductor,
            [Service] IBreweryOrderConductor orderConductor,
            [Service] IMapper mapper,
            CancellationToken cancellationToken)
        {
            if (!validationConductor.CanSearch(state, type, out var errors))
            {
                foreach (var (key, message) in errors)
                {
                    ctx.ReportError(
                        ErrorBuilder.New()
                            .SetCode(key)
                            .SetPath(ctx.Path)
                            .AddLocation(ctx.FieldSelection)
                            .SetMessage(message)
                            .Build()
                    );
                }

                return null;
            }

            Expression<Func<Brewery, bool>> filter;

            if (!string.IsNullOrEmpty(breweryId?.Trim()))
            {
                filter = (b) => b.BreweryId == breweryId;
            }
            else
            {
                filter = filterConductor.BuildFilter(
                    by_name: name,
                    by_state: state,
                    by_type: type,
                    by_city: city,
                    by_tags: tags);
            }

            if (!string.IsNullOrEmpty(search))
            {
                filter = filter.AndAlso(filterConductor
                    .BuildFilter(by_name: search));
            }

            // Sorting
            Func<IQueryable<Brewery>, IQueryable<Brewery>> orderBy = null;
            if (sort != null)
            {
                orderBy = orderConductor.OrderByFields(
                    sort?
                        .Select(s => s.FirstOrDefault() == '-'
                            ? new KeyValuePair<string, SortDirection>(s.Substring(1), SortDirection.DESC)
                            : new KeyValuePair<string, SortDirection>(s, SortDirection.ASC))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                );
            }

            var result = breweryConductor.FindAllQueryable(filter: filter, orderBy: orderBy);

            if (!result.HasErrorsOrResultIsNull())
            {
                return result.ResultObject;
            }

            foreach (var err in result.Errors)
            {
                ctx.ReportError(
                    ErrorBuilder.New()
                        .SetCode(err.Key)
                        .SetPath(ctx.Path)
                        .AddLocation(ctx.FieldSelection)
                        .SetMessage(err.Message)
                        .Build()
                );
            }

            return null;
        }
    }
}
