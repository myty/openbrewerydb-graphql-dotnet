using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AndcultureCode.CSharp.Core.Models.Entities;
using HotChocolate;
using OpenBreweryDB.Data.Models.Favorites;
using OpenBreweryDB.Data.Models.Reviews;

namespace OpenBreweryDB.Data.Models.Users
{
    public class User : Entity, IKeyedEntity
    {
        [GraphQLNonNullType]
        public string FirstName { get; set; }

        [GraphQLNonNullType]
        public string LastName { get; set; }

        [GraphQLNonNullType]
        public string Email { get; set; }

        [GraphQLIgnore]
        public string PasswordHash { get; set; }

        [GraphQLIgnore]
        public string Salt { get; set; }

        [GraphQLIgnore]
        public List<Favorite> Favorites { get; set; }

        public List<Review> UserReviews { get; set; }

        // Computed Property
        public string FullName => string.Join(" ",
            new string[] { FirstName, LastName }
                .Select(s => s?.Trim())
                .Where(s => !string.IsNullOrEmpty(s)));
    }
}
