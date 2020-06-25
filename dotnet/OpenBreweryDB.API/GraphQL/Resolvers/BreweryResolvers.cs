using AndcultureCode.CSharp.Core.Extensions;
using AutoMapper;
using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using OpenBreweryDB.API.GraphQL.Types;
using OpenBreweryDB.Core.Conductors;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTO = OpenBreweryDB.Core.Models;
using Entity = OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.API.GraphQL.Resolvers
{
    public static class BreweryResolvers
    {
        public static async Task<Entity.Brewery> BreweryNodeResolver(IResolverContext ctx, long id)
        {
            var breweryConductor = ctx.Service<IBreweryConductor>();

            var breweryResult = breweryConductor.Find(id);

            if (!breweryResult.HasErrorsOrResultIsNull())
            {
                return await Task.FromResult(breweryResult.ResultObject);
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
    }
}
