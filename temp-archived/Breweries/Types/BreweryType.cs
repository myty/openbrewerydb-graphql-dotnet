using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.DataLoader;
using GraphQL.Relay.Types;
using GraphQL.Types;
using OpenBreweryDB.API.GraphQL.Breweries.Dataloaders;
using OpenBreweryDB.API.GraphQL.Resolvers;
using OpenBreweryDB.API.GraphQL.Reviews.Dataloaders;
using OpenBreweryDB.Data.Models;

namespace OpenBreweryDB.API.GraphQL.Types
{
    public class BreweryType : NodeGraphType<Brewery>
    {
        readonly IBreweryStore _breweryStore;

        public BreweryType(IDataLoaderContextAccessor accessor, IBreweryStore breweryStore)
        {
            _breweryStore = breweryStore;

            Name = "Brewery";
            Description = "A brewery of beer";

            Id(t => t.Id);

            Field(d => d.BreweryType, nullable: false)
                .Name("brewery_type")
                .Description("Type of Brewery");

            Field(t => t.BreweryId, nullable: false)
                .Name("brewery_id")
                .Description("Friendly id for Brewery");

            Field(t => t.City)
                .Name("city")
                .Description("The city of the brewery");

            Field(t => t.Country)
                .Name("country")
                .Description("The country of origin for the brewery");

            Field(t => t.Longitude)
                .Name("longitude")
                .Description("Longitude portion of lat/long coordinates");

            Field(t => t.Latitude)
                .Name("latitude")
                .Description("Latitude portion of lat/long coordinates");

            Field(t => t.Name)
                .Name("name")
                .Description("Name of brewery");

            Field(t => t.Phone)
                .Name("phone")
                .Description("The phone number for the brewery");

            Field(t => t.PostalCode)
                .Name("postal_code")
                .Description("The state of the brewery");

            Field(t => t.State)
                .Name("state")
                .Description("The state of the brewery");

            Field(t => t.Street)
                .Name("street")
                .Description("The street of the brewery");

            Field(t => t.UpdatedAt, nullable: false)
                .Name("updated_at")
                .Description("Date timestamp of the last time the record was updated");

            Field(t => t.WebsiteURL)
                .Name("website_url")
                .Description("Website address for the brewery");

            Field<NonNullGraphType<ListGraphType<StringGraphType>>>("tag_list", resolve: (context) =>
            {
                return context.Source?.BreweryTags?
                    .Select(bt => bt.Tag.Name)
                    .Distinct()
                    .ToList() ?? new List<string>();
            });
        }

        public override Brewery GetById(string breweryId) {
            return _breweryStore.GetBreweryById(long.Parse(breweryId)).ResultObject;
        }

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

            descriptor.Field("reviews")
                .Resolver(ctx =>
                {
                    var brewery = ctx.Parent<Brewery>();

                    return ctx.DataLoader<ReviewsByBreweryIdDataLoader>()
                        .LoadAsync(brewery.Id, ctx.RequestAborted);
                });
        }
    }
}
