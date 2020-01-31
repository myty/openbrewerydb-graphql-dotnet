using AutoMapper;
using GraphQL;
using GraphQL.Types;
using OpenBreweryDB.API.GraphQL.Types;
using OpenBreweryDB.Core.Conductors;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

using DTO = OpenBreweryDB.Core.Models;
using Entity = OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.API.GraphQL.Queries
{
    public class BreweriesQuery : ObjectGraphType
    {
        readonly IBreweryConductor _breweryConductor;
        readonly IBreweryFilterConductor _filterConductor;
        readonly IMapper _mapper;
        readonly IBreweryOrderConductor _orderConductor;
        readonly IBreweryValidationConductor _validationConductor;

        public BreweriesQuery(IBreweryConductor breweryConductor, IBreweryFilterConductor filterConductor, IBreweryOrderConductor orderConductor, IMapper mapper, IBreweryValidationConductor validationConductor)
        {
            _breweryConductor = breweryConductor;
            _filterConductor = filterConductor;
            _mapper = mapper;
            _orderConductor = orderConductor;
            _validationConductor = validationConductor;

            Name = nameof(BreweriesQuery);

            Field<ListGraphType<BreweryType>>(
                "breweries",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "name", Description = "name of the brewery" },
                    new QueryArgument<StringGraphType> { Name = "city", Description = "city of the brewery" },
                    new QueryArgument<StringGraphType> { Name = "state", Description = "state of the brewery" },
                    new QueryArgument<StringGraphType> { Name = "type", Description = "type of brewery" },
                    new QueryArgument<StringGraphType> { Name = "search", Description = "search for name or partial name of all breweries" },
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "tags", Description = "tag associated w/ brewery" },
                    new QueryArgument<IntGraphType> { Name = "skip", Description = "number of records to skip" },
                    new QueryArgument<IntGraphType> { Name = "limit", Description = "number of return records to limit" },
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "sort", Description = "sort by (name, type, city, state)" }
                ),
                resolve: GetBreweries
            );

            Field<BreweryType>(
                "brewery",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id", Description = "id of the brewery (required)" }
                ),
                resolve: GetBrewery
            );
        }

        private DTO.Brewery GetBrewery(ResolveFieldContext<object> context)
        {
            var id = context.GetArgument<long>("id");

            var breweryResult = _breweryConductor.Find(id);

            if (breweryResult.HasErrors || breweryResult.ResultObject is null)
            {
                context.Errors.AddRange(breweryResult.Errors.Select(err => new ExecutionError(err.Message)));
                return null;
            }

            return _mapper.Map<DTO.Brewery>(breweryResult.ResultObject);
        }

        private IEnumerable<DTO.Brewery> GetBreweries(ResolveFieldContext<object> context)
        {
            var by_state = context.GetArgument<string>("state");
            var by_type = context.GetArgument<string>("type");

            if (!_validationConductor.CanSearch(by_state, by_type, out var errors))
            {
                context.Errors.AddRange(errors.Select(err => new ExecutionError(err)));
                return null;
            }

            var filter = _filterConductor.BuildFilter(
                by_name: context.GetArgument<string>("name"),
                by_state: by_state,
                by_type: by_type,
                by_city: context.GetArgument<string>("city"),
                by_tags: context.GetArgument<IEnumerable<string>>("tags"));

            if (context.HasArgument("search"))
            {
                var searchFilter = _filterConductor.BuildFilter(
                    by_name: context.GetArgument<string>("search"));

                filter = filter.AndAlso(searchFilter);
            }

            // Sorting
            Func<IQueryable<Entity.Brewery>, IQueryable<Entity.Brewery>> orderBy = null;
            if (context.HasArgument("sort"))
            {
                orderBy = _orderConductor.OrderByFields(
                    context.GetArgument<IEnumerable<string>>("sort")?
                        .Select(s => {
                            if (s.FirstOrDefault() == '-')
                            {
                                return new KeyValuePair<string, SortDirection>(s.Substring(1), SortDirection.DESC);
                            }

                            return new KeyValuePair<string, SortDirection>(s, SortDirection.ASC);
                        })
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                );
            }

            var skip = context.GetArgument<int>("skip", 0);
            var limit = context.GetArgument<int>("limit", 10);

            var result = _breweryConductor.FindAll(filter: filter, orderBy: orderBy, skip: skip, take: limit);

            if (result.HasErrors || result.ResultObject is null)
            {
                context.Errors.AddRange(result.Errors.Select(err => new ExecutionError(err.Message)));
                return null;
            }

            return _mapper.Map<IEnumerable<DTO.Brewery>>(result.ResultObject);
        }
    }
}
