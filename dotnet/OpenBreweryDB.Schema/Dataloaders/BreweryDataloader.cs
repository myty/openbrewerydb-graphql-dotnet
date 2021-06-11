using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AndcultureCode.CSharp.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.Schema.Dataloaders
{
    public class BreweryDataloader
    {
        private readonly IBreweryConductor _breweryConductor;

        public BreweryDataloader(IBreweryConductor breweryConductor)
        {
            _breweryConductor = breweryConductor;
        }

        public async Task<IDictionary<long, Brewery>> GetBreweryById(
            IEnumerable<long> ids,
            CancellationToken cancellationToken) =>
            await _breweryConductor
                .FindAllQueryable(t => ids.Contains(t.Id))
                .ThrowIfAnyErrors()
                .ResultObject
                .ToDictionaryAsync(c => c.Id);
    }
}
