using HotChocolate;
using HotChocolate.Types;
using OpenBreweryDB.Data.Models;
using OpenBreweryDB.Data.Models.Reviews;

namespace OpenBreweryDB.API.GraphQL.Reviews
{
    [ExtendObjectType(Name = "Subscription")]
    public class ReviewSubscriptions
    {
        [Subscribe(With = "reviews")]
        public Review OnReview(
            [EventMessage] Review review) =>
            review;
    }
}
