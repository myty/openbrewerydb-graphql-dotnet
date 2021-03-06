using System;
using System.Linq;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using OpenBreweryDB.API.GraphQL.Breweries.Dataloaders;
using OpenBreweryDB.API.GraphQL.Resolvers;
using OpenBreweryDB.API.GraphQL.Reviews.Dataloaders;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.API.GraphQL.Types
{
    public class BreweryType : ObjectType<Brewery>
    {
        protected override void Configure(IObjectTypeDescriptor<Brewery> descriptor)
        {
            descriptor
                .Name("Brewery")
                .Description("A brewery of beer")
                .ImplementsNode().IdField(t => t.Id).ResolveNode((ctx, id) => ctx
                    .DataLoader<BreweryByIdDataLoader>()
                    .LoadAsync(id, ctx.RequestAborted));

            descriptor.Field("nearby")
                .Argument(
                    "within",
                    a => a
                        .Type<IntType>()
                        .Description("limit the nearby breweries to search radius, defaults to 25 milles")
                )
                .ResolveWith<BreweryResolvers>(r => r.GetNearbyBreweriesAsync(default));

            descriptor.Field(t => t.BreweryType)
                .Type<NonNullType<StringType>>()
                .Name("brewery_type")
                .Description("Type of Brewery");

            descriptor.Field(t => t.BreweryId)
                .Type<NonNullType<StringType>>()
                .Name("brewery_id")
                .Description("Friendly id for Brewery");

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
                .Type<StringType>()
                .Description("The state of the brewery");

            descriptor.Field(t => t.Street)
                .Type<StringType>()
                .Description("The street of the brewery");

            descriptor.Field(t => t.BreweryTags).Ignore();

            descriptor.Field("tag_list")
                .Type<NonNullType<ListType<StringType>>>()
                .Description("Tags that have been attached to the brewery")
                .Resolver(ctx =>
                {
                    return ctx.Parent<Brewery>()?.BreweryTags?
                        .Select(bt => bt.Tag.Name)
                        .Distinct() ?? Array.Empty<string>();
                });

            descriptor.Field(t => t.BreweryReviews).Ignore();

            descriptor.Field("reviews")
                .Resolver(ctx =>
                {
                    var brewery = ctx.Parent<Brewery>();

                    return ctx.DataLoader<ReviewsByBreweryIdDataLoader>()
                        .LoadAsync(brewery.Id, ctx.RequestAborted);
                });

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
