using HotChocolate;
using HotChocolate.Types.Relay;
using OpenBreweryDB.API.GraphQL.Common;
using OpenBreweryDB.Data.Models;
using System.Collections.Generic;

namespace OpenBreweryDB.API.GraphQL.Breweries
{
    public class UpdateBreweryInput : InputBase
    {
        public UpdateBreweryInput(
            long id,
            string clientMutationId)
            : base(clientMutationId)
        {
            Id = id;
        }

        [ID(nameof(Brewery))]
        public long Id { get; }

        [GraphQLDescription("Name of brewery")]
        [GraphQLNonNullType]
        public string Name { get; set; }

        [GraphQLName("brewery_type")]
        [GraphQLDescription("Type of Brewery")]
        [GraphQLNonNullType]
        public string BreweryType { get; set; }

        [GraphQLName("brewery_id")]
        [GraphQLNonNullType]
        public string BreweryId { get; set; }

        [GraphQLDescription("The street of the brewery")]
        public string Street { get; set; }

        [GraphQLDescription("The city of the brewery")]
        public string City { get; set; }

        [GraphQLDescription("The state of the brewery")]
        [GraphQLNonNullType]
        public string State { get; set; }

        [GraphQLName("postal_code")]
        [GraphQLDescription("The postal code of the brewery")]
        public string PostalCode { get; set; }

        [GraphQLName("website_url")]
        [GraphQLDescription("Website address for the brewery")]
        public string WebsiteURL { get; set; }

        [GraphQLDescription("The phone number for the brewery")]
        public string Phone { get; set; }

        [GraphQLDescription("The country of origin for the brewery")]
        public string Country { get; set; }

        [GraphQLDescription("Longitude portion of lat/long coordinates")]
        public decimal? Longitude { get; set; }

        [GraphQLDescription("Latitude portion of lat/long coordinates")]
        public decimal? Latitude { get; set; }

        [GraphQLName("tag_list")]
        [GraphQLDescription("Tags that have been attached to the brewery")]
        public IEnumerable<string> Tags { get; set; }
    }
}
