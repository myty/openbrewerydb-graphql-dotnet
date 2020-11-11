using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using HotChocolate.DataLoader;
using Microsoft.EntityFrameworkCore;
using OpenBreweryDB.Data;
using OpenBreweryDB.Data.Models.Reviews;

namespace OpenBreweryDB.API.GraphQL.Reviews.Dataloaders
{
    public class ReviewsByBreweryIdDataLoader : BatchDataLoader<long, IEnumerable<Review>>
    {
        private readonly BreweryDbContext _breweryDbContext;

        public ReviewsByBreweryIdDataLoader(
            IBatchScheduler batchScheduler,
            BreweryDbContext breweryDbContext)
            : base(batchScheduler)
        {
            _breweryDbContext = breweryDbContext ?? throw new ArgumentNullException(nameof(breweryDbContext));
        }

        protected override async Task<IReadOnlyDictionary<long, IEnumerable<Review>>> LoadBatchAsync(
            IReadOnlyList<long> keys,
            CancellationToken cancellationToken)
        {
            var breweryReviews = await _breweryDbContext.Reviews
                .Where(r => keys.Contains(r.BreweryId))
                .ToListAsync();

            return breweryReviews.GroupBy(r => r.BreweryId)
                .ToDictionary(g => g.Key, g => g.AsEnumerable());
        }
    }
}
