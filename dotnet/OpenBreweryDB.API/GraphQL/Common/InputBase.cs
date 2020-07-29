using HotChocolate;

namespace OpenBreweryDB.API.GraphQL.Common
{
    public class InputBase
    {
        public InputBase(string clientMutationId)
        {
            ClientMutationId = clientMutationId;
        }

        [GraphQLDescription("Relay Client Mutation Id")]
        [GraphQLNonNullType]
        public string ClientMutationId { get; }
    }
}
