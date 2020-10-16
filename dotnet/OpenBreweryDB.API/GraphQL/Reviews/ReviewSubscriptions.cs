using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using OpenBreweryDB.Data.Models.Reviews;

namespace OpenBreweryDB.API.GraphQL.Reviews
{
    [ExtendObjectType(Name = "Subscription")]
    public class ReviewSubscriptions
    {
        [SubscribeAndResolve]
        public async ValueTask<ISourceStream<Review>> OnReviewReceivedAsync(
            [Service] ITopicEventReceiver eventReceiver,
            CancellationToken cancellationToken)
        {
            return await eventReceiver.SubscribeAsync<string, Review>("reviews", cancellationToken);
        }
    }
}
