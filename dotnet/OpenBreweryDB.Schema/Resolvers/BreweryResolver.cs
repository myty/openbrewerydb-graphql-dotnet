using System.Linq;
using AndcultureCode.CSharp.Core.Interfaces;
using AndcultureCode.CSharp.Core.Models.Errors;
using GraphQL;
using GraphQL.Builders;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.Schema.Resolvers
{
    public class BreweryResolver
    {
        private readonly IBreweryConductor _breweryConductor;

        public BreweryResolver(IBreweryConductor breweryConductor)
        {
            _breweryConductor = breweryConductor;
        }

        public IResult<IQueryable<Brewery>> ResolveBreweries(IResolveFieldContext context) => _breweryConductor
            .FindAllQueryable()
            .Include(
                nameof(Brewery.BreweryTags),
                context.ContainsField("tag_list"));

        public IResult<IQueryable<Brewery>> ResolveNearbyBreweries(IResolveConnectionContext<Brewery> context)
        {
            if (context.Source.Latitude.HasValue &&
                context.Source.Longitude.HasValue)
            {
                var latitude = context.Source.Latitude.Value;
                var longitude = context.Source.Longitude.Value;
                var within = context.GetArgument("within", 25);

                return _breweryConductor
                    .FindAllByLocation(
                        latitude,
                        longitude,
                        within)
                    .Include(
                        nameof(Brewery.BreweryTags),
                        context.ContainsField("tag_list"));
            }

            return new Result<IQueryable<Brewery>>(Enumerable.Empty<Brewery>().AsQueryable());
        }
    }
}
