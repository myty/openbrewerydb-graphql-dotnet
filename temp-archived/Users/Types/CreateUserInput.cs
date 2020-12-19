using OpenBreweryDB.API.GraphQL.Common;

namespace OpenBreweryDB.API.GraphQL.Users.Types
{
    public class CreateUserInput : ClientMutationBase
    {
        public CreateUserInput(
            string email,
            string firstName,
            string lastName,
            string password,
            string clientMutationId)
            : base(clientMutationId)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
        }

        public string Email { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string Password { get; }
    }
}
