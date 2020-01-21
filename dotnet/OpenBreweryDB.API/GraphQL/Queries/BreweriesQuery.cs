using System.Linq;
using AutoMapper;
using GraphQL.Types;
using OpenBreweryDB.API.GraphQL.Types;
using DTO = OpenBreweryDB.Core.Models;
using Entity = OpenBreweryDB.Data.Models;
using OpenBreweryDB.Data;
using System.Collections.Generic;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System;

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
                    new QueryArgument<StringGraphType> { Name = "search", Description = "search for name or partial name of all breweries" },
                    new QueryArgument<StringGraphType> { Name = "tag", Description = "tag associated w/ brewery" }
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
                    by_tag: context.GetArgument<string>("tag"));
            }

            var result = _data.Breweries
                .Include(b => b.BreweryTags)
                    .ThenInclude(bt => bt.Tag)
                .Where(filter);

            return _mapper.Map<IEnumerable<DTO.Brewery>>(result);
        }
    }
}