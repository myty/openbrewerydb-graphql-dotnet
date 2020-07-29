namespace OpenBreweryDB.API.GraphQL.Common
{
    public class InputBase
    {
        public InputBase(string? clientMutationId)
        {
            ClientMutationId = clientMutationId;
        }

        public string? ClientMutationId { get; }
    }
}
