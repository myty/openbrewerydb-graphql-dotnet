using OpenBreweryDB.API.GraphQL.Common;

namespace OpenBreweryDB.API.GraphQL.Users.Types
{
    public class LoginInput : ClientMutationBase
    {
        public LoginInput(
            string email,
            string password,
            string clientMutationId)
            : base(clientMutationId)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; }

        public string Password { get; }
    }
}
