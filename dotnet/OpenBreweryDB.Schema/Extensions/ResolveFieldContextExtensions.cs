using System;
using System.Linq;

namespace GraphQL
{
    public static class ResolveFieldContextExtensions
    {
        public static bool ContainsField(this IResolveFieldContext context, string name) =>
            context.SubFields.Keys.Contains(name);
    }
}
