using GraphQL.Types;

namespace OpenBreweryDB.Schema.Types
{
    public class NodeInterface : InterfaceGraphType
    {
        public NodeInterface()
        {
            Name = "Node";

            Field<IdGraphType>("id", "Global node Id");
        }
    }
}
