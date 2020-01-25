using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GraphQL.Types;
using OpenBreweryDB.API.GraphQL.InputTypes;
using OpenBreweryDB.API.GraphQL.Types;
using OpenBreweryDB.Data;
using OpenBreweryDB.Core.Extensions;

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
                resolve: CreateBrewery
            );

            Field<BreweryType>(
                "updateBrewery",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<BreweryInputType>> { Name = "brewery" }
                ),
                resolve: UpdateBrewery
            );

            Field<StringGraphType>(
                "deleteBrewery",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id" }),
                resolve: context =>
                {
                    var breweryId = context.GetArgument<long>("id");
                    var brewery = _data.Breweries.Find(breweryId);

                    _data.Remove<Entity.Brewery>(brewery);
                    _data.SaveChanges();

                    return $"The brewery with the id: {breweryId} has been successfully deleted.";
                }
            );
        }

        private DTO.Brewery CreateBrewery(ResolveFieldContext<object> context)
        {
            var dto = context.ConvertContextToObject<DTO.Brewery>(ConvertBrewery);
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

            return _mapper.Map<DTO.Brewery>(brewery);
        }

        private DTO.Brewery UpdateBrewery(ResolveFieldContext<object> context)
        {
            var dto = context.ConvertContextToObject<DTO.Brewery>(ConvertBrewery);

            var brewery = _mapper.Map<Entity.Brewery>(dto);

            var now = DateTime.Now;
            brewery.UpdatedAt = now;

            _data.Breweries.Update(brewery);
            _data.SaveChanges();

            return _mapper.Map<DTO.Brewery>(brewery);
        }

        private DTO.Brewery ConvertBrewery(ResolveFieldContext<object> context)
        {
            var dto = context.GetArgument<DTO.Brewery>("brewery");
            var breweryContext = context.Arguments["brewery"] as IDictionary<string, object>;

            dto.BreweryType = breweryContext.ContainsKey("brewery_type")
                ? breweryContext["brewery_type"] as string
                : null;

            dto.PostalCode = breweryContext.ContainsKey("postal_code")
                ? breweryContext["postal_code"] as string
                : null;

            dto.WebsiteURL = breweryContext.ContainsKey("website_url")
                ? breweryContext["website_url"] as string
                : null;

            dto.Tags = breweryContext.ContainsKey("tag_list")
                ? (breweryContext["tag_list"] as List<object>).AsEnumerable().Select(s => s as string)
                : Enumerable.Empty<string>();

            return dto;
        }
    }
}
