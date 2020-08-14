using HotChocolate;
using HotChocolate.Types.Relay;
using OpenBreweryDB.API.GraphQL.Common;
using OpenBreweryDB.Core.Models;

namespace OpenBreweryDB.API.GraphQL.Reviews.Types
{
    public class ReviewInput : ClientMutationBase
    {
        public ReviewInput(
            long breweryId,
            string subject,
            string body,
            string clientMutationId) : base(clientMutationId)
        {
            Subject = subject;
            Body = body;
            BreweryId = breweryId;
        }

        [GraphQLNonNullType]
        public string Subject { get; }

        [GraphQLNonNullType]
        public string Body { get; }

        [ID(nameof(Brewery))]
        [GraphQLNonNullType]
        public long BreweryId { get; }
    }
}
