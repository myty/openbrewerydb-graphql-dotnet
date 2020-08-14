using AndcultureCode.CSharp.Core.Models.Entities;
using HotChocolate;
using OpenBreweryDB.Data.Models.Users;

namespace OpenBreweryDB.Data.Models.Favorites
{
    public class Favorite : Entity, IKeyedEntity
    {
        [GraphQLIgnore]
        public long BreweryId { get; set; }

        [GraphQLNonNullType]
        public Brewery Brewery { get; set; }

        [GraphQLIgnore]
        public long UserId { get; set; }

        [GraphQLNonNullType]
        public User User { get; set; }
    }
}
