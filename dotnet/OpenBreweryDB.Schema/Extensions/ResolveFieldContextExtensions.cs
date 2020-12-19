using System.Linq;
using GraphQL.Language.AST;

namespace GraphQL
{
    public static class ResolveFieldContextExtensions
    {
        public static bool FieldSetContains(this IResolveFieldContext context, string name) => context
            .FieldAst.SelectionSet.Selections
                .Any(s => s.As<Field>()?.Name == name);
    }
}
