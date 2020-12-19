using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using AndcultureCode.CSharp.Core.Extensions;
using HotChocolate;
using HotChocolate.Resolvers;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.API.GraphQL.Breweries
{
    public partial class BreweryQueries
    {
        public Brewery GetBreweryById(
            [GraphQLDescription("filter by brewery id"), GraphQLName("brewery_id")] string breweryId,
            IResolverContext ctx,
            [Service] IBreweryConductor breweryConductor,
            CancellationToken cancellationToken)
        {
            Expression<Func<Brewery, bool>> filter;

            if (!string.IsNullOrEmpty(breweryId?.Trim()))
            {
                filter = (b) => b.BreweryId == breweryId;
            }
            else
            {
                filter = (b) => false;
            }

            var result = breweryConductor.FindAllQueryable(filter: filter);

            if (!result.HasErrorsOrResultIsNull())
            {
                return result.ResultObject.FirstOrDefault();
            }

            foreach (var err in result.Errors)
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
