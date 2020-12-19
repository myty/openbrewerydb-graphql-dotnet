using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AndcultureCode.CSharp.Core.Extensions;
using AndcultureCode.CSharp.Core.Interfaces;
using AndcultureCode.CSharp.Core.Models.Errors;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.API.GraphQL.Breweries.Dataloaders
{
    public class BreweryStore : IBreweryStore
    {
        private readonly IBreweryConductor _breweryConductor;

        public BreweryStore(
            IBreweryConductor breweryConductor)
        {
            _breweryConductor = breweryConductor ?? throw new ArgumentNullException(nameof(breweryConductor));
        }

        public IResult<Brewery> GetBreweryById(long id)
        {
            return _breweryConductor
                .Find(id)
                .ThrowIfAnyErrorsOrResultIsNull();
        }

        public async Task<IResult<IDictionary<long, Brewery>>> GetBreweriesByIdAsync(IEnumerable<long> breweryIds, CancellationToken cancellationToken)
        {
            var breweryResult = _breweryConductor
                .FindAll(b => breweryIds.Contains(b.Id))
                .ThrowIfAnyErrorsOrResultIsNull();

            var dictionaryResult = breweryIds
                .ToDictionary(
                    i => i,
                    i => breweryResult.ResultObject.FirstOrDefault(r => r.Id == i));

            return await Task.FromResult(
                new Result<IDictionary<long, Brewery>>(dictionaryResult));
        }

        public async Task<IEnumerable<Brewery>> GetNearbyBreweriesAsync(IResolverContext ctx)
        {
            var brewery = ctx.Parent<Brewery>();

            if (!brewery.Latitude.HasValue || !brewery.Longitude.HasValue)
            {
                return Enumerable.Empty<Brewery>();
            }

            var breweryConductor = ctx.Service<IBreweryConductor>();
            var mileRadius = ctx.ArgumentValue<int?>("within") ?? 25;
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
