using System.Linq;
using GraphQL.Types;

using DTO = OpenBreweryDB.Core.Models;

namespace OpenBreweryDB.API.GraphQL.InputTypes
{
    public class BreweryInputType : InputObjectGraphType<DTO.Brewery>
    {
        public BreweryInputType()
        {
            Name = "BreweryInput";
            Description = "The brewery to create.";

            Field(d => d.Id, nullable: true).Description("The id of the brewery.");
            Field(d => d.BreweryType, nullable: false).Name("brewery_type").Description("Which type of brewery is it.");
            Field(d => d.City, nullable: true).Description("The city of the brewery.");
            Field(d => d.Country, nullable: true).Description("The country of origin for the brewery.");
            Field(d => d.Longitude, nullable: true).Description("Longtitude portion of lat/long coordinates");
            Field(d => d.Latitude, nullable: true).Description("Latitude portion of lat/long coordinates");
            Field(d => d.Name, nullable: false).Description("The name of the brewery.");
            Field(d => d.Phone, nullable: true).Description("The phone number for the brewery.");
            Field(d => d.PostalCode, nullable: true).Name("postal_code").Description("The state of the brewery.");
            Field(d => d.State, nullable: false).Description("The state of the brewery.");
            Field(d => d.Street, nullable: true).Description("The street of the brewery.");

            Field(d => d.Tags)
                .Name("tag_list")
                .Description("Tags that have been attached to the brewery.")
                .DefaultValue(Enumerable.Empty<string>());

            Field(d => d.WebsiteURL, nullable: true).Name("website_url").Description("Webiste address for the brewery.");
        }
    }
}
