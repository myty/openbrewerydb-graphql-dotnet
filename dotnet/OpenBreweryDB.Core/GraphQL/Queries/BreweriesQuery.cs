using System;
using GraphQL.Types;
using OpenBreweryDB.Core.GraphQL.Types;
using OpenBreweryDB.Data;

namespace OpenBreweryDB.Core
{
    public class BreweriesQuery : ObjectGraphType
    {
        public BreweriesQuery(BreweryDbContext data)
        {
            Name = nameof(BreweriesQuery);

            // Field<CharacterInterface>("hero", resolve: context => data.GetDroidByIdAsync("3"));
            // Field<HumanType>(
            //     "human",
            //     arguments: new QueryArguments(
            //         new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the human" }
            //     ),
            //     resolve: context => data.GetHumanByIdAsync(context.GetArgument<string>("id"))
            // );

            // Func<ResolveFieldContext, string, object> func = (context, id) => data.GetDroidByIdAsync(id);

            // FieldDelegate<DroidType>(
            //     "droid",
            //     arguments: new QueryArguments(
            //         new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the droid" }
            //     ),
            //     resolve: func
            // );
        }
    }
}