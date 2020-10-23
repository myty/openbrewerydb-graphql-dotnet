using HotChocolate;
using HotChocolate.Types;
using OpenBreweryDB.Data.Models;
using OpenBreweryDB.Data.Models.Reviews;

namespace OpenBreweryDB.API.GraphQL.Reviews
{
    [ExtendObjectType(Name = "Subscription")]
    public class ReviewSubscriptions
    {
        [Subscribe]
        public Review OnReviewReceived(
            [EventMessage] Review review) =>
            review;
    }
}
