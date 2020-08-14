using AndcultureCode.CSharp.Core.Models.Entities;
using HotChocolate;
using OpenBreweryDB.Data.Models.Favorites;
using OpenBreweryDB.Data.Models.Reviews;
using System;
using System.Collections.Generic;

namespace OpenBreweryDB.Data.Models
{
    public class Brewery : Entity, IKeyedEntity
    {
        public string BreweryId { get; set; }
        public string Name { get; set; }
        public string BreweryType { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string WebsiteURL { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Country { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }

        public List<BreweryTag> BreweryTags { get; set; }

        [GraphQLIgnore]
        public List<Favorite> FavoriteUsers { get; set; }

        public List<Review> BreweryReviews { get; set; }
    }
}
