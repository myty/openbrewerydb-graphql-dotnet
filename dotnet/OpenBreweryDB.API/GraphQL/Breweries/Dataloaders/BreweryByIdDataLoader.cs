using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.DataLoader;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Data.Models;
using AndcultureCode.CSharp.Core.Extensions;
using System.Linq;
using OpenBreweryDB.API.GraphQL.Errors;

namespace OpenBreweryDB.API.GraphQL.Breweries.Dataloaders
{
    public class BreweryByIdDataLoader : BatchDataLoader<long, Brewery>
    {
        private readonly IBreweryConductor _breweryConductor;

        public BreweryByIdDataLoader(IBreweryConductor breweryConductor)
        {
            _breweryConductor = breweryConductor ?? throw new ArgumentNullException(nameof(breweryConductor));
        }

        protected override async Task<IReadOnlyDictionary<long, Brewery>> LoadBatchAsync(
            IReadOnlyList<long> keys,
            CancellationToken cancellationToken)
        {
            var breweryResult = _breweryConductor.FindAll(b => keys.Contains(b.Id));

            if (!breweryResult.HasErrorsOrResultIsNull())
            {
                return await Task.FromResult(breweryResult.ResultObject.ToDictionary(t => t.Id));
            }

            if (breweryResult.Errors?.Any() == true)
            {
                throw new ResultException(breweryResult.Errors);
            }

            return null;
        }
    }
}
