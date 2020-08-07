using HotChocolate;
using OpenBreweryDB.API.GraphQL.Common;
using System.Collections.Generic;

namespace OpenBreweryDB.API.GraphQL.Breweries
{
    public class CreateBreweryInput : InputBase
    {
        public CreateBreweryInput(
            string name,
            string breweryType,
            string clientMutationId)
            : base(clientMutationId)
        {
            Name = name;
            BreweryType = breweryType;
        }

        [GraphQLDescription("Name of brewery")]
        [GraphQLNonNullType]
        public string Name { get; }

        [GraphQLName("brewery_type")]
        [GraphQLDescription("Type of Brewery")]
        [GraphQLNonNullType]
        public string BreweryType { get; }

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
