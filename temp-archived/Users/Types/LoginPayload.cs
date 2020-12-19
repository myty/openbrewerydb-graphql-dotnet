using OpenBreweryDB.API.GraphQL.Common;
using OpenBreweryDB.Data.Models.Users;

namespace OpenBreweryDB.API.GraphQL.Users.Types
{
    public class LoginPayload : ClientMutationBase
    {
        public LoginPayload(
            User me,
            string token,
            string scheme,
            string clientMutationId) : base(clientMutationId)
        {
            Me = me;
            Token = token;
            Scheme = scheme;
        }

        public User Me { get; }

        public string Token { get; }

        public string Scheme { get; }
    }
}
