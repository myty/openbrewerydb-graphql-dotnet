using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Types;
using GraphQL.Types.Relay;
using Panic.StringUtils;

namespace OpenBreweryDB.Schema.Types
{
    public interface IRelayNode
    {
        object GetById(string id);
    }

    public interface IRelayNode<out TOut>
    {
        TOut GetById(string id);
    }

    public static class Node
    {
        public static string ToGlobalId(string name, object id) => StringUtils.Base64Encode($"{name}:{id}");

        public static FieldBuilder<TSourceType, TReturnType> ResolveByGlobalId<TSourceType, TReturnType>(this FieldBuilder<TSourceType, TReturnType> fieldBuilder) => fieldBuilder.Resolve(context =>
        {
            var globalId = context.GetArgument<string>("id");
            return context.GetByGlobalId<TReturnType>(globalId);
        });

        private static T GetByGlobalId<T>(this IResolveFieldContext context, string globalId)
        {
            var parts = StringUtils.Base64Decode(globalId).Split(':');
            var type = parts.FirstOrDefault();

            return context.Schema.AllTypes[type] is IRelayNode<T> node ?
                node.GetById(parts.LastOrDefault()) :
                default;
        }
    }

    public abstract class NodeGraphType<T, TOut> : ObjectGraphType<T>, IRelayNode<TOut>, IRelayNode
    {
        public static Type Edge => typeof(EdgeType<NodeGraphType<T, TOut>>);

        public static Type Connection => typeof(ConnectionType<NodeGraphType<T, TOut>>);

        protected NodeGraphType()
        {
            Interface<NodeInterface>();
        }

        public abstract TOut GetById(string id);

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

        object IRelayNode.GetById(string id) => GetById(id);
    }

    public abstract class NodeGraphType<TSource> : NodeGraphType<TSource, TSource>
    {
    }

    public abstract class NodeGraphType : NodeGraphType<object>
    {
    }

    public abstract class AsyncNodeGraphType<TSource> : NodeGraphType<TSource, Task<TSource>>
    {
    }
}
