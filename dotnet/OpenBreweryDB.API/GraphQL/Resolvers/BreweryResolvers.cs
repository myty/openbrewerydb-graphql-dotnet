using AndcultureCode.CSharp.Core.Extensions;
using HotChocolate;
using HotChocolate.Resolvers;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Data.Models;
using System.Threading.Tasks;

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
    }
}
