using AutoMapper;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using OpenBreweryDB.API.GraphQL.Types;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Core.Extensions;
using OpenBreweryDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DTO = OpenBreweryDB.Core.Models;
using Entity = OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.API.GraphQL.Queries
{
    public class BreweriesQuery : ObjectGraphType
    {
        readonly BreweryDbContext _data;
        readonly IMapper _mapper;
        readonly IBreweryFilterConductor _filterConductor;

        public BreweriesQuery(BreweryDbContext data, IMapper mapper, IBreweryFilterConductor filterConductor)
        {
            _data = data;
            _mapper = mapper;
            _filterConductor = filterConductor;

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
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "tags", Description = "tag associated w/ brewery" }
                ),
                resolve: GetBreweries
            );
        }

        private IEnumerable<DTO.Brewery> GetBreweries(ResolveFieldContext<object> context)
        {
            Expression<Func<Entity.Brewery, bool>> filter = null;

            if (context.HasArgument("id"))
            {
                var id = context.GetArgument<long>("id");

                filter = b => b.BreweryId == id;
            }
            else
            {
                filter = _filterConductor.BuildFilter(
                    by_name: context.GetArgument<string>("name"),
                    by_state: context.GetArgument<string>("state"),
                    by_type: context.GetArgument<string>("type"),
                    by_tags: context.GetArgument<IEnumerable<string>>("tags"));

                if (context.HasArgument("search"))
                {
                    var searchFilter = _filterConductor.BuildFilter(
                        by_name: context.GetArgument<string>("search"));

                    filter = filter.AndAlso(searchFilter);
                }
            }

            var result = _data.Breweries
                .Include(b => b.BreweryTags)
                    .ThenInclude(bt => bt.Tag)
                .Where(filter);

            return _mapper.Map<IEnumerable<DTO.Brewery>>(result);
        }
    }
}