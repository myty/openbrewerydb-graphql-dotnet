using System;
using System.Collections.Generic;
using GraphQL.Types;
using Newtonsoft.Json.Linq;

namespace OpenBreweryDB.Core.Extensions
{
    public static class ResolveFieldContextExtensions
    {
        public static TObject ConvertContextToObject<TObject>(
            this ResolveFieldContext<object> context,
            Func<ResolveFieldContext<object>, TObject> process) => process(context);

        public static TObject ConvertArgumentToObject<TObject>(this ResolveFieldContext ctx, string argumentName)
            => _convertArgument<TObject>(ctx.Arguments, argumentName);

        public static TObject ConvertArgumentToObject<TObject>(this ResolveFieldContext<object> ctx, string argumentName)
            => _convertArgument<TObject>(ctx.Arguments, argumentName);

        public static TObject ConvertArgumentToObject<TObject, TSource>(this ResolveFieldContext<TSource> ctx, string argumentName)
            => _convertArgument<TObject>(ctx.Arguments, argumentName);

        private static TObject _convertArgument<TObject>(IReadOnlyDictionary<string, object> arguments, string argumentName)
        {
            return arguments[argumentName] != null
                ? JToken.FromObject(arguments[argumentName]).ToObject<TObject>()
                : default;
        }
    }
}
