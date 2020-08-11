using System.Linq;
using System;
using AndcultureCode.CSharp.Core.Extensions;
using HotChocolate;
using HotChocolate.Resolvers;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Data.Models;
using System.Threading.Tasks;
using GeoCoordinatePortable;
using System.Collections.Generic;

namespace OpenBreweryDB.API.GraphQL.Resolvers
{
    public static class BreweryResolvers
    {
        public static async Task<Brewery> BreweryNodeResolver(IResolverContext ctx, long id)
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

        public static async Task<IEnumerable<Brewery>> NearbyBreweriesResolver(IResolverContext ctx)
        {
            var brewery = ctx.Parent<Brewery>();

            if (!brewery.Latitude.HasValue || !brewery.Longitude.HasValue)
            {
                return Enumerable.Empty<Brewery>();
            }

            var breweryConductor = ctx.Service<IBreweryConductor>();
            var mileRadius = ctx.Argument<int?>("within") ?? 25;
            var breweryResult = breweryConductor
                .FindAllByLocation(
                    Convert.ToDouble(brewery.Latitude),
                    Convert.ToDouble(brewery.Longitude),
                    mileRadius);

            if (!breweryResult.HasErrorsOrResultIsNull())
            {
                return await Task.FromResult(breweryResult.ResultObject.Where(b => b.Id != brewery.Id));
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
