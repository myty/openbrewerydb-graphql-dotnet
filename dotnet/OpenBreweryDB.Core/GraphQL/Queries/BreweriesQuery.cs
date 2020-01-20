using System;
using System.Linq;
using AutoMapper;
using GraphQL.Types;
using OpenBreweryDB.Core.GraphQL.Types;
using DTO = OpenBreweryDB.Core.Model;
using OpenBreweryDB.Data;

namespace OpenBreweryDB.Core
{
    public class BreweriesQuery : ObjectGraphType
    {
        readonly BreweryDbContext _data;
        readonly IMapper _mapper;

        public BreweriesQuery(BreweryDbContext data, IMapper mapper)
        {
            _data = data;
            _mapper = mapper;

            Name = nameof(BreweriesQuery);

            Field<BreweryType>(
                "breweries",
                // arguments: new QueryArguments(
                //     new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the brewery" }
                // ),
                resolve: GetBrewery
            );
        }

        private DTO.Brewery GetBreweries(ResolveFieldContext<object> context)
        {
            var id = context.GetArgument<long?>("id");

            var result = _data.Breweries
                .Where(b => b.BreweryId == id)
                .FirstOrDefault();

            return _mapper.Map<DTO.Brewery>(result);
        }

        private DTO.Brewery GetBrewery(ResolveFieldContext<object> context)
        {
            var id = context.GetArgument<long>("id");
            var result = _data.Breweries
                .Where(b => b.BreweryId == id)
                .FirstOrDefault();

            return _mapper.Map<DTO.Brewery>(result);
        }
    }
}