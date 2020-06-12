using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;
using HotChocolate.Types;
using DTO = OpenBreweryDB.Core.Models;

namespace OpenBreweryDB.API.GraphQL.InputTypes
{
    public class BreweryInputType : InputObjectType<DTO.Brewery>
    {
        protected override void Configure(IInputObjectTypeDescriptor<DTO.Brewery> descriptor)
        {
            descriptor.Field(t => t.BreweryType)
                .Type<NonNullType<StringType>>()
                .Name("brewery_type")
                .Description("Type of Brewery");

            descriptor.Field(t => t.Id)
                .Type<IdType>()
                .Description("Brewery Unique Identifier");

            descriptor.Field(t => t.City)
                .Type<StringType>()
                .Description("The city of the brewery");

            descriptor.Field(t => t.Country)
                .Type<StringType>()
                .Description("The country of origin for the brewery");

            descriptor.Field(t => t.Longitude)
                .Type<DecimalType>()
                .Description("Longitude portion of lat/long coordinates");

            descriptor.Field(t => t.Latitude)
                .Type<DecimalType>()
                .Description("Latitude portion of lat/long coordinates");

            descriptor.Field(t => t.Name)
                .Type<NonNullType<StringType>>()
                .Description("Name of brewery");

            descriptor.Field(t => t.Phone)
                .Type<StringType>()
                .Description("The phone number for the brewery");

            descriptor.Field(t => t.PostalCode)
                .Type<StringType>()
                .Name("postal_code")
                .Description("The state of the brewery");

            descriptor.Field(t => t.State)
                .Type<NonNullType<StringType>>()
                .Description("The state of the brewery");

            descriptor.Field(t => t.Street)
                .Type<StringType>()
                .Description("The street of the brewery");

            descriptor.Field(t => t.Tags)
                .Type<NonNullType<ListType<StringType>>>()
                .DefaultValue(Enumerable.Empty<string>())
                .Name("tag_list")
                .Description("Tags that have been attached to the brewery");

            descriptor.Field(t => t.UpdatedAt)
                .Type<NonNullType<DateTimeType>>()
                .Name("updated_at")
                .Description("Date timestamp of the last time the record was updated");

            descriptor.Field(t => t.WebsiteURL)
                .Type<StringType>()
                .Name("website_url")
                .Description("Website address for the brewery");
        }
    }
}
