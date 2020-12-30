using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Types;
using OpenBreweryDB.Data.Models;
using OpenBreweryDB.Schema.Dataloaders;
using OpenBreweryDB.Schema.Resolvers;

namespace OpenBreweryDB.Schema.Types
{
    public class BreweryType : AsyncNodeGraphType<Brewery>
    {
        private readonly IDataLoader<long, Brewery> _loader;

        public BreweryType(
            BreweryResolver breweryResolver,
            TagResolver tagResolver,
            IDataLoaderContextAccessor accessor,
            BreweryDataloader breweryDataloader)
        {
            _loader = accessor.Context
                .GetOrAddBatchLoader<long, Brewery>(
                    "GetTagById",
                    breweryDataloader.GetBreweryById);

            Name = "Brewery";
            Description = "A brewery of beer";

            Id(d => d.Id);

            Field(d => d.BreweryType, nullable: false)
                .Name("brewery_type")
                .Description("Type of Brewery");

            Field(t => t.BreweryId, nullable: false)
                .Name("external_id")
                .Description("External identifier for Brewery");

            Field(t => t.City)
                .Name("city")
                .Description("The city of the brewery");

            Field(t => t.Country)
                .Name("country")
                .Description("The country of origin for the brewery");

            Field<DecimalGraphType, decimal?>("longitude")
                .Resolve((context) => context.Source.Longitude)
                .Description("Longitude portion of lat/long coordinates");

            Field<DecimalGraphType, decimal?>("latitude")
                .Resolve((context) => context.Source.Latitude)
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

            Field<NonNullGraphType<ListGraphType<StringGraphType>>, IEnumerable<string>>()
                .Name("tag_list")
                .ResolveAsync(tagResolver.ResolveTagListAsync);

            Connection<BreweryType>()
                .Name("nearby")
                .Argument<IntGraphType, int>("within", "search radius in miles", 25)
                .ResolveQueryableResult(breweryResolver.ResolveNearbyBreweries);

            // TODO: reviews
        }

        public override async Task<Brewery> GetById(string id) =>
            await _loader.LoadAsync(long.Parse(id)).GetResultAsync();
    }
}
