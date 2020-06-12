using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using OpenBreweryDB.API.GraphQL.InputTypes;
using OpenBreweryDB.API.GraphQL.Types;
using OpenBreweryDB.Data;
using OpenBreweryDB.Core.Extensions;

using DTO = OpenBreweryDB.Core.Models;
using Entity = OpenBreweryDB.Data.Models;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using HotChocolate.Types;
using HotChocolate.Resolvers;
using HotChocolate;

namespace OpenBreweryDB.API.GraphQL.Mutations
{
    public class BreweriesMutation : ObjectType
    {
        protected override void Configure(IObjectTypeDescriptor descriptor)
        {
            descriptor
                .Field("createBrewery")
                .Type<BreweryType>()
                .Argument("brewery", a => a.Type<NonNullType<BreweryInputType>>())
                .Resolver(CreateBrewery);

            descriptor
                .Field("updateBrewery")
                .Type<BreweryType>()
                .Argument("brewery", a => a.Type<NonNullType<BreweryInputType>>())
                .Resolver(UpdateBrewery);

            descriptor
                .Field("deleteBrewery")
                .Type<StringType>()
                .Argument("id", a => a.Type<NonNullType<IdType>>())
                .Resolver(DeleteBrewery);
        }

        private DTO.Brewery CreateBrewery(IResolverContext context)
        {
            var dto = context.Argument<DTO.Brewery>("brewery");
            var validationConductor = context.Service<IBreweryValidationConductor>();
            var breweryConductor = context.Service<IBreweryConductor>();
            var mapper = context.Service<IMapper>();

            if (!validationConductor.CanCreate(dto, out var errors))
            {
                foreach (var (key, message) in errors)
                {
                    context.ReportError(
                        ErrorBuilder.New()
                            .SetCode(key)
                            .SetPath(context.Path)
                            .AddLocation(context.FieldSelection)
                            .SetMessage(message)
                            .Build()
                    );
                }
                return null;
            }

            var brewery = breweryConductor.Create(mapper.Map<Entity.Brewery>(dto));

            if (brewery.HasErrors || brewery.ResultObject is null)
            {
                foreach (var err in brewery.Errors)
                {
                    context.ReportError(
                        ErrorBuilder.New()
                            .SetCode(err.Key)
                            .SetPath(context.Path)
                            .AddLocation(context.FieldSelection)
                            .SetMessage(err.Message)
                            .Build()
                    );
                }
                return null;
            }

            return mapper.Map<DTO.Brewery>(brewery.ResultObject);
        }

        private DTO.Brewery UpdateBrewery(IResolverContext context)
        {
            var dto = context.Argument<DTO.Brewery>("brewery");
            var validationConductor = context.Service<IBreweryValidationConductor>();
            var breweryConductor = context.Service<IBreweryConductor>();
            var mapper = context.Service<IMapper>();

            if (!validationConductor.CanUpdate(dto.Id ?? default, dto, out var errors))
            {
                foreach (var (key, message) in errors)
                {
                    context.ReportError(
                        ErrorBuilder.New()
                            .SetCode(key)
                            .SetPath(context.Path)
                            .AddLocation(context.FieldSelection)
                            .SetMessage(message)
                            .Build()
                    );
                }
                return null;
            }

            var brewery = breweryConductor.Update(mapper.Map<Entity.Brewery>(dto));

            if (brewery.HasErrors || brewery.ResultObject is null)
            {
                foreach (var err in brewery.Errors)
                {
                    context.ReportError(
                        ErrorBuilder.New()
                            .SetCode(err.Key)
                            .SetPath(context.Path)
                            .AddLocation(context.FieldSelection)
                            .SetMessage(err.Message)
                            .Build()
                    );
                }
                return null;
            }

            return mapper.Map<DTO.Brewery>(brewery.ResultObject);
        }

        private string DeleteBrewery(IResolverContext context)
        {
            var breweryId = context.Argument<long>("id");
            //var validationConductor = context.Service<IBreweryValidationConductor>();
            var breweryConductor = context.Service<IBreweryConductor>();
            //var mapper = context.Service<IMapper>();

            var deleteResult = breweryConductor.Delete(breweryId);

            if (deleteResult.HasErrors || !deleteResult.ResultObject)
            {
                foreach (var err in deleteResult.Errors)
                {
                    context.ReportError(
                        ErrorBuilder.New()
                            .SetCode(err.Key)
                            .SetPath(context.Path)
                            .AddLocation(context.FieldSelection)
                            .SetMessage(err.Message)
                            .Build()
                    );
                }
                return null;
            }

            return $"The brewery with the id: {breweryId} has been successfully deleted.";
        }
    }
}
