using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GraphQL.Types;
using OpenBreweryDB.API.GraphQL.InputTypes;
using OpenBreweryDB.API.GraphQL.Types;
using OpenBreweryDB.Core.Extensions;

using DTO = OpenBreweryDB.Core.Models;
using Entity = OpenBreweryDB.Data.Models;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using GraphQL;

namespace OpenBreweryDB.API.GraphQL.Mutations
{
    public class BreweriesMutation : ObjectGraphType
    {
        readonly IBreweryConductor _breweryConductor;
        readonly IMapper _mapper;
        readonly IBreweryValidationConductor _validationConductor;

        public BreweriesMutation(IBreweryConductor breweryConductor, IMapper mapper, IBreweryValidationConductor validationConductor)
        {
            _breweryConductor = breweryConductor;
            _mapper = mapper;
            _validationConductor = validationConductor;

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

                    var deleteResult = _breweryConductor.Delete(breweryId);

                    if (deleteResult.HasErrors || !deleteResult.ResultObject)
                    {
                        context.Errors.AddRange(deleteResult.Errors.Select(err => new ExecutionError(err.Message)));
                        return null;
                    }

                    return $"The brewery with the id: {breweryId} has been successfully deleted.";
                }
            );
        }

        private DTO.Brewery CreateBrewery(ResolveFieldContext<object> context)
        {
            var dto = context.ConvertContextToObject<DTO.Brewery>(ConvertBrewery);

            if (!_validationConductor.CanCreate(dto, out var errors))
            {
                context.Errors.AddRange(errors.Select(err => new ExecutionError(err)));
                return null;
            }

            var brewery = _breweryConductor.Create(_mapper.Map<Entity.Brewery>(dto));

            if (brewery.HasErrors || brewery.ResultObject is null)
            {
                context.Errors.AddRange(brewery.Errors.Select(err => new ExecutionError(err.Message)));
                return null;
            }

            return _mapper.Map<DTO.Brewery>(brewery.ResultObject);
        }

        private DTO.Brewery UpdateBrewery(ResolveFieldContext<object> context)
        {
            var dto = context.ConvertContextToObject<DTO.Brewery>(ConvertBrewery);

            if (!_validationConductor.CanUpdate(dto.Id ?? default, dto, out var errors))
            {
                context.Errors.AddRange(errors.Select(err => new ExecutionError(err)));
                return null;
            }

            var brewery = _breweryConductor.Update(_mapper.Map<Entity.Brewery>(dto));

            if (brewery.HasErrors || brewery.ResultObject is null)
            {
                context.Errors.AddRange(brewery.Errors.Select(err => new ExecutionError(err.Message)));
                return null;
            }

            return _mapper.Map<DTO.Brewery>(brewery.ResultObject);
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
