using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Data.Models;
using AndcultureCode.CSharp.Core.Extensions;
using System.Linq;
using OpenBreweryDB.API.GraphQL.Errors;
using GreenDonut;
using ACModels = AndcultureCode.CSharp.Core.Models;
using AndcultureCode.CSharp.Core.Enumerations;

namespace OpenBreweryDB.API.GraphQL.Breweries.Dataloaders
{
    public class BreweryByIdDataLoader : DataLoaderBase<long, Brewery>
    {
        private readonly IBreweryConductor _breweryConductor;

        public BreweryByIdDataLoader(IBreweryConductor breweryConductor)
        {
            _breweryConductor = breweryConductor ?? throw new ArgumentNullException(nameof(breweryConductor));
        }

        protected override async Task<IReadOnlyList<Result<Brewery>>> FetchAsync(
            IReadOnlyList<long> keys,
            CancellationToken cancellationToken)
        {
            var breweryResult = _breweryConductor.FindAll(b => keys.Contains(b.Id));

            if (!breweryResult.HasErrorsOrResultIsNull())
            {
                return breweryResult.ResultObject
                    .Select(Result<Brewery>.Resolve)
                    .ToList();
            }

            if (breweryResult.Errors?.Any() == true)
            {
                return breweryResult
                    .Errors
                    .Select(e => Result<Brewery>.Reject(new ResultException(e)))
                    .ToList();
            }

            return await Task.FromResult(new List<Result<Brewery>> {
                Result<Brewery>.Reject(new ResultException(new ACModels.Error {
                    ErrorType = ErrorType.Error,
                    Key = "NotFound",
                    Message = "Resource Not Found"
                }))
            });
        }
    }
}
