using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AndcultureCode.CSharp.Core.Interfaces;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.API.GraphQL.Breweries.Dataloaders
{
    public interface IBreweryStore
    {
        IResult<Brewery> GetBreweryById(long id);

        // This will be called by the loader for all pending keys
        // Note that fetch delegates can accept a CancellationToken parameter or not
        Task<IResult<IDictionary<long, Brewery>>> GetBreweriesByIdAsync(IEnumerable<long> breweryIds, CancellationToken cancellationToken);

        Task<IEnumerable<Brewery>> GetNearbyBreweriesAsync(IResolverContext ctx);
    }
}
