using GraphQL.Types;

namespace OpenBreweryDB.API.GraphQL.Common
{
    public class ClientMutationBase
    {
        public ClientMutationBase(string clientMutationId)
        {
            ClientMutationId = clientMutationId;
        }

        public string ClientMutationId { get; }
    }

    public class ClientMutationBaseType : InterfaceGraphType<ClientMutationBase>
    {
        public ClientMutationBaseType()
        {
            Name = "ClientMutation";
            Description = "Base for all client mutations.";
            Field(d => d.ClientMutationId, nullable: false).Description("Relay Client Mutation Id");
        }
    }
}
