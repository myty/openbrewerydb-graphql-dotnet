using AutoMapper;
using HotChocolate.Types;
using OpenBreweryDB.API.GraphQL.Types;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using AndcultureCode.CSharp.Core.Extensions;
using HotChocolate;
using DTO = OpenBreweryDB.Core.Models;
using Entity = OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.API.GraphQL.Queries
{
    public class BreweriesQuery : ObjectType
    {
        protected override void Configure(IObjectTypeDescriptor descriptor)
        {
            descriptor.Field("brewery")
                .Type<BreweryType>()
                .Argument(
                    "id",
                    a => a
                        .Type<NonNullType<IdType>>()
                        .Description("id of the brewery (required)")
                )
                .Resolver(
                    ctx =>
                    {
                        var breweryId        = ctx.Argument<long>("id");
                        var breweryConductor = ctx.Service<IBreweryConductor>();
                        var mapper           = ctx.Service<IMapper>();

                        var breweryResult = breweryConductor.Find(breweryId);

                        if (!breweryResult.HasErrorsOrResultIsNull())
                        {
                            return mapper.Map<DTO.Brewery>(breweryResult.ResultObject);
                        }

                        foreach (var err in breweryResult.Errors)
                        {
                            ctx.ReportError(
                                ErrorBuilder.New()
                                    .SetCode(err.Key)
                                    .SetPath(ctx.Path)
                                    .AddLocation(ctx.FieldSelection)
                                    .SetMessage(err.Message)
                                    .Build()
                            );
                        }

                        return null;
                    }
                );

            // TODO: Create GetBreweries Resolver
            // descriptor.Field(t => t.GetBreweries(default, default, default))
        }
    }
}
