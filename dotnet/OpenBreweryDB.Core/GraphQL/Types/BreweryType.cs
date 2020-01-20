using GraphQL.Types;
using OpenBreweryDB.Core.Model;

namespace OpenBreweryDB.Core.GraphQL.Types
{
    public class BreweryType : ObjectGraphType<Brewery>
    {
        public BreweryType()
        {
            Name = "Brewery";
            Description = "A brewery of beer.";

            Field(d => d.Name, nullable: false).Description("The name of the brewery.");
            Field(d => d.Tags, nullable: false).Name("tags").Description("Tags that have been attached to the brewery.");
            Field<BreweryTypeEnum>("type", "Which type of brewery is it.");
        }
    }
}