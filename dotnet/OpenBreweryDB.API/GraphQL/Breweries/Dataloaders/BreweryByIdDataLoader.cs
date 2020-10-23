using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AndcultureCode.CSharp.Core.Enumerations;
using AndcultureCode.CSharp.Core.Extensions;
using GreenDonut;
using HotChocolate.DataLoader;
using OpenBreweryDB.API.GraphQL.Errors;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Data.Models;
using ACErrors = AndcultureCode.CSharp.Core.Models.Errors;

namespace OpenBreweryDB.API.GraphQL.Breweries.Dataloaders
{
    public class BreweryByIdDataLoader : BatchDataLoader<long, Brewery>
    {
        private readonly IBreweryConductor _breweryConductor;

        public BreweryByIdDataLoader(
            IBatchScheduler batchScheduler,
            IBreweryConductor breweryConductor)
            : base(batchScheduler)
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
                return breweryResult.ResultObject
                    .Select(Result<Brewery>.Resolve)
                    .ToDictionary(t => t.Value.Id, t => t.Value);
            }

            if (breweryResult.Errors?.Any() == true)
            {
                return breweryResult
                    .Errors
                    .Select(e => Result<Brewery>.Reject(new ResultException(e)))
                    .ToDictionary(t => t.Value.Id, t => t.Value);
            }

            return await Task.FromResult(new List<Result<Brewery>> {
                Result<Brewery>.Reject(new ResultException(new ACErrors.Error {
                    ErrorType = ErrorType.Error,
                    Key = "NotFound",
                    Message = "Resource Not Found"
                }))
            }.ToDictionary(t => t.Value.Id, t => t.Value));
        }
    }
}
