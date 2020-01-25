using GraphQL.Types;

using DTO = OpenBreweryDB.Core.Models;

namespace OpenBreweryDB.API.GraphQL.Types
{
    public class BreweryType : ObjectGraphType<DTO.Brewery>
    {
        public BreweryType()
        {
            Name = "Brewery";
            Description = "A brewery of beer.";

            Field(d => d.BreweryType, nullable: false).Name("brewery_type").Description("Which type of brewery is it.");
            Field(d => d.City, nullable: true).Description("The city of the brewery.");
            Field(d => d.Country, nullable: true).Description("The country of origin for the brewery.");
            Field(d => d.Id, nullable: true).Description("The id of the brewery.");
            Field(d => d.Longitude, nullable: true).Description("Longtitude portion of lat/long coordinates");
            Field(d => d.Latitude, nullable: true).Description("Latitude portion of lat/long coordinates");
            Field(d => d.Name, nullable: false).Description("The name of the brewery.");
            Field(d => d.Phone, nullable: true).Description("The phone number for the brewery.");
            Field(d => d.PostalCode, nullable: true).Name("postal_code").Description("The state of the brewery.");
            Field(d => d.State, nullable: false).Description("The state of the brewery.");
            Field(d => d.Street, nullable: true).Description("The street of the brewery.");
            Field(d => d.Tags, nullable: false).Name("tag_list").Description("Tags that have been attached to the brewery.");

            Field<NonNullGraphType<DateTimeGraphType>>(
                name: "updated_at",
                description: "Date timestamp of the last time the record was updated",
                resolve: ctx => ctx.Source.UpdatedAt);

            Field(d => d.WebsiteURL, nullable: true).Name("website_url").Description("Webiste address for the brewery.");
        }
    }
}
