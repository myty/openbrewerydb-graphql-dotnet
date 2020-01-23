using System;
using System.Linq;
using AutoMapper;
using GraphQL.Types;
using OpenBreweryDB.API.GraphQL.InputTypes;
using OpenBreweryDB.API.GraphQL.Types;
using OpenBreweryDB.Data;

using DTO = OpenBreweryDB.Core.Models;
using Entity = OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.API.GraphQL.Mutations
{
    public class BreweriesMutation : ObjectGraphType
    {
        readonly BreweryDbContext _data;
        readonly IMapper _mapper;

        public BreweriesMutation(BreweryDbContext data, IMapper mapper)
        {
            _data = data;
            _mapper = mapper;

            Field<BreweryType>(
                "createBrewery",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<BreweryInputType>> { Name = "brewery" }
                ),
                resolve: context =>
                {
                    var dto = context.GetArgument<DTO.Brewery>("brewery");
                    var brewery = _mapper.Map<Entity.Brewery>(dto);

                    var now = DateTime.Now;
                    brewery.CreatedAt = now;
                    brewery.UpdatedAt = now;

                    var existingTags = _data.Tags
                        .Where(t => dto.Tags.Contains(t.Name))
                        .ToList();

                    var existingTagNames = existingTags
                        .Select(t => t.Name)
                        .ToList();

                    var tagsToCreate = dto.Tags
                        .Where(t => !existingTagNames.Contains(t))
                        .Select(t => new Entity.Tag { Name = t })
                        .ToList();

                    var breweryTags = tagsToCreate
                        .Concat(existingTags)
                        .Select(t => new Entity.BreweryTag
                        {
                            Brewery = brewery,
                            Tag = t
                        })
                        .ToList();

                    _data.BreweryTags.AddRange(breweryTags);
                    _data.Tags.AddRange(tagsToCreate);
                    _data.Breweries.Add(brewery);

                    _data.SaveChanges();

                    return brewery;
                });
        }
    }
}