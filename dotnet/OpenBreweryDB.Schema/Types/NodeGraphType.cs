using System;
using System.Linq;
using System.Linq.Expressions;
using GraphQL;
using GraphQL.Types;
using GraphQL.Types.Relay;
using Panic.StringUtils;

namespace OpenBreweryDB.Schema.Types
{
    public interface IRelayNode
    {
        object ConvertToId(string id);

        object GetById(object id);
    }

    public interface IRelayNode<TId, TOut>
    {
        TId ParseId(string id);

        TOut GetById(TId id);
    }

    public static class Node
    {
        public static string ToGlobalId(string name, object id) => StringUtils.Base64Encode($"{name}:{id}");

        public static object GetByGlobalId(this IResolveFieldContext context)
        {
            var globalId = context.GetArgument<string>("id");
            var parts = StringUtils.Base64Decode(globalId).Split(':');
            var type = parts.FirstOrDefault();
            var node = (IRelayNode)context.Schema.FindType(type);
            var id = node.ConvertToId(parts.LastOrDefault());

            return node.GetById(id);
        }
    }

    public abstract class NodeGraphType<T, TId> : ObjectGraphType<T>, IRelayNode<TId, T>, IRelayNode
    {
        public static Type Edge => typeof(EdgeType<NodeGraphType<T, TId>>);

        public static Type Connection => typeof(ConnectionType<NodeGraphType<T, TId>>);

        protected NodeGraphType()
        {
            Interface<NodeInterface>();
        }

        public abstract TId ParseId(string id);

        public abstract T GetById(TId id);

        public FieldType Id<TReturnType>(Expression<Func<T, TReturnType>> expression)
        {
            var name = StringUtils.ToCamelCase(expression.NameOf());

            return Id(name, expression);
        }

        public FieldType Id<TReturnType>(string name, Expression<Func<T, TReturnType>> expression)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                // if there is a field called "ID" on the object, namespace it to "contactId"
                if (name.ToLower() == "id")
                {
                    if (string.IsNullOrWhiteSpace(Name))
                        throw new InvalidOperationException(
                            "The parent GraphQL type must define a Name before declaring the Id field " +
                            "in order to properly prefix the local id field");

                    name = StringUtils.ToCamelCase(Name + "Id");
                }

                Field(name, expression)
                    .Description($"The Id of the {Name ?? "node"}")
                    .FieldType.Metadata["RelayLocalIdField"] = true;
            }

            var field = Field(
                name: "id",
                description: $"The Global Id of the {Name ?? "node"}",
                type: typeof(NonNullGraphType<IdGraphType>),
                resolve: context => Node.ToGlobalId(
                    context.ParentType.Name,
                    expression.Compile()(context.Source)
                )
            );

            field.Metadata["RelayGlobalIdField"] = true;

            if (!string.IsNullOrWhiteSpace(name))
            {
                field.Metadata["RelayRelatedLocalIdField"] = name;
            }

            return field;
        }

        object IRelayNode.ConvertToId(string id) => ParseId(id);

        public object GetById(object id)
        {
            if (!(this is IRelayNode<TId, T> node) || !(id is TId nodeId))
            {
                throw new InvalidOperationException();
            }

            return node.GetById(nodeId);
        }
    }
}
