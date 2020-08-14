using OpenBreweryDB.API.GraphQL.Common;
using OpenBreweryDB.Data.Models.Reviews;

namespace OpenBreweryDB.API.GraphQL.Reviews.Types
{
    public class ReviewPayload : ClientMutationBase
    {
        public ReviewPayload(
            Review review,
            string clientMutationId) : base(clientMutationId)
        {
            Review = review;
        }

        public Review Review { get; }
    }
}
