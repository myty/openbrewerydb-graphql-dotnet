using AutoMapper;
using GraphQL;
using GraphQL.Types;
using OpenBreweryDB.API.GraphQL.Types;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Core.Extensions;
using System.Collections.Generic;
using System.Linq;
using DTO = OpenBreweryDB.Core.Models;

namespace OpenBreweryDB.API.GraphQL.Queries
{
    public class BreweriesQuery : ObjectGraphType
    {
        readonly IBreweryConductor _breweryConductor;
        readonly IBreweryFilterConductor _filterConductor;
        readonly IMapper _mapper;
        readonly IBreweryValidationConductor _validationConductor;

        public BreweriesQuery(IBreweryConductor breweryConductor, IBreweryFilterConductor filterConductor, IMapper mapper, IBreweryValidationConductor validationConductor)
        {
            _breweryConductor = breweryConductor;
            _filterConductor = filterConductor;
            _mapper = mapper;
            _validationConductor = validationConductor;

            Name = nameof(BreweriesQuery);

            Field<ListGraphType<BreweryType>>(
                "breweries",
                arguments: new QueryArguments(
                    new QueryArgument<IdGraphType> { Name = "id", Description = "id of the brewery" },
                    new QueryArgument<StringGraphType> { Name = "name", Description = "name of the brewery" },
                    new QueryArgument<StringGraphType> { Name = "city", Description = "city of the brewery" },
                    new QueryArgument<StringGraphType> { Name = "state", Description = "state of the brewery" },
                    new QueryArgument<StringGraphType> { Name = "type", Description = "type of brewery" },
                    new QueryArgument<StringGraphType> { Name = "search", Description = "search for name or partial name of all breweries" },
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "tags", Description = "tag associated w/ brewery" },
                    new QueryArgument<IntGraphType> { Name = "skip", Description = "number of records to skip" },
                    new QueryArgument<IntGraphType> { Name = "limit", Description = "number of return records to limit" }
                ),
                resolve: GetBreweries
            );
        }

        private IEnumerable<DTO.Brewery> GetBreweries(ResolveFieldContext<object> context)
        {
            if (context.HasArgument("id"))
            {
                var id = context.GetArgument<long>("id");

                var brewery = _breweryConductor.Find(id);

                return new List<DTO.Brewery> { _mapper.Map<DTO.Brewery>(brewery) };
            }

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
                by_tags: context.GetArgument<IEnumerable<string>>("tags"));

            if (context.HasArgument("search"))
            {
                var searchFilter = _filterConductor.BuildFilter(
                    by_name: context.GetArgument<string>("search"));

                filter = filter.AndAlso(searchFilter);
            }

            var skip = context.GetArgument<int>("skip", 0);
            var limit = context.GetArgument<int>("limit", 10);

            var result = _breweryConductor.FindAll(filter: filter, skip: skip, take: limit);

            return _mapper.Map<IEnumerable<DTO.Brewery>>(result);
        }
    }
}
