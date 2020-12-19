using System.Collections.Generic;
using System.Linq;
using AndcultureCode.CSharp.Core.Extensions;
using AndcultureCode.CSharp.Core.Interfaces;
using GraphQL;
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

        public IEnumerable<Brewery> ResolveBreweries(IResolveFieldContext context) => _breweryConductor
            .FindAllQueryable()
            .Include(
                nameof(Brewery.BreweryTags),
                context.FieldSetContains("tag_list"))
            .Take(
                context.GetArgument("take", 25))
            .ThrowIfAnyErrorsOrResultIsNull()
            .ResultObject;

        public IEnumerable<Brewery> ResolveNearbyBreweries(IResolveFieldContext<Brewery> context)
        {
            if (context.Source.Latitude.HasValue &&
                context.Source.Longitude.HasValue)
            {
                var latitude = context.Source.Latitude.Value;
                var longitude = context.Source.Longitude.Value;

                return _breweryConductor
                    .FindAllByLocation(
                        latitude,
                        longitude,
                        context.GetArgument("within", 25))
                    .Include(
                        nameof(Brewery.BreweryTags),
                        context.FieldSetContains("tag_list"))
                    .ThrowIfAnyErrorsOrResultIsNull()
                    .ResultObject;
            }

            return Enumerable.Empty<Brewery>();
        }
    }
}
