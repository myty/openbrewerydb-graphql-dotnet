using AndcultureCode.CSharp.Core.Models.Entities;
using OpenBreweryDB.Data.Models.Users;

namespace OpenBreweryDB.Data.Models.Favorites
{
    public class Favorite : Entity, IKeyedEntity
    {
        public new long Id { get; set; }

        public long BreweryId { get; set; }

        public Brewery Brewery { get; set; }

        public long UserId { get; set; }

        public User User { get; set; }
    }
}
