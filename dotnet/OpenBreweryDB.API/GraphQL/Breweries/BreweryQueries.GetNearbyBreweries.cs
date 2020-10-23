using System.Linq;
using System.Threading;
using AndcultureCode.CSharp.Core.Extensions;
using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.API.GraphQL.Breweries
{
    public partial class BreweryQueries
    {
        [UsePaging]
        public IQueryable<Brewery> GetNearbyBreweries(
            double latitude,
            double longitude,
            IResolverContext ctx,
            [Service] IBreweryConductor breweryConductor,
            CancellationToken cancellationToken)
        {
            var result = breweryConductor.FindAllByLocation(latitude, longitude);

            if (!result.HasErrorsOrResultIsNull())
            {
                return result.ResultObject;
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
