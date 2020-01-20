using System;
using System.Linq;
using AutoMapper;
using GraphQL.Types;
using OpenBreweryDB.Core.GraphQL.Types;
using DTO = OpenBreweryDB.Core.Models;
using OpenBreweryDB.Data;
using System.Collections.Generic;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace OpenBreweryDB.Core
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

            Field<BreweryType>(
                "brewery",
                arguments: new QueryArguments(
                    new QueryArgument<LongGraphType> { Name = "id", Description = "id of the brewery" }
                ),
                resolve: GetBrewery
            );

            Field<ListGraphType<BreweryType>>(
                "breweries",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "name", Description = "name of the brewery" },
                    new QueryArgument<StringGraphType> { Name = "tag", Description = "tag associated w/ brewery" }
                ),
                resolve: GetBreweries
            );
        }

        private IList<DTO.Brewery> GetBreweries(ResolveFieldContext<object> context)
        {
            var filter = _filterConductor.BuildFilter(
                by_name: context.GetArgument<string>("name"),
                by_tag: context.GetArgument<string>("tag"));

            var result = _data.Breweries
                .Include(b => b.BreweryTags)
                    .ThenInclude(bt => bt.Tag)
                .Where(filter);

            return _mapper.Map<IList<DTO.Brewery>>(result);
        }

        private DTO.Brewery GetBrewery(ResolveFieldContext<object> context)
        {
            var id = context.GetArgument<long>("id");
            var result = _data.Breweries
                .Include(b => b.BreweryTags)
                    .ThenInclude(bt => bt.Tag)
                .Where(b => b.BreweryId == id)
                .FirstOrDefault();

            var brewery = _mapper.Map<DTO.Brewery>(result);

            return brewery;
        }
    }
}