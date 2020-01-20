using GraphQL.Types;
using OpenBreweryDB.Core.Models;

namespace OpenBreweryDB.Core.GraphQL.Types
{
    public class BreweryType : ObjectGraphType<Brewery>
    {
        public BreweryType()
        {
            Name = "Brewery";
            Description = "A brewery of beer.";

            Field(d => d.Name, nullable: false).Description("The name of the brewery.");
            Field(d => d.Tags, nullable: false).Name("tag_list").Description("Tags that have been attached to the brewery.");
            Field(d => d.BreweryType, nullable: false).Name("brewery_type").Description("Which type of brewery is it.");
        }
    }
}