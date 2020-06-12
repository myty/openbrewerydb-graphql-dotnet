using AutoMapper;
using HotChocolate.Types;
using OpenBreweryDB.API.GraphQL.Types;
using OpenBreweryDB.Core.Conductors;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

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
                .Argument("id", a => a
                    .Type<NonNullType<IdType>>()
                    .Description("id of the brewery (required)"))
                .Resolver(ctx =>
                {
                    var breweryId = ctx.Argument<long>("id");
                    var query = ctx.Service<Query>();
                    var breweryResult = query.GetBrewery(breweryId);

                    // TODO: Add error handling

                    return breweryResult.ResultObject;
                });

            // TODO: Create GetBreweries Resolver
            // descriptor.Field(t => t.GetBreweries(default, default, default))
        }
    }
}
