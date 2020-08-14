using OpenBreweryDB.API.GraphQL.Common;
using OpenBreweryDB.Data.Models.Users;

namespace OpenBreweryDB.API.GraphQL.Users.Types
{
    public class CreateUserPayload : ClientMutationBase
    {
        public CreateUserPayload(User user, string clientMutationId) : base(clientMutationId)
        {
            User = user;
        }

        public User User { get; }
    }
}
