using System.Collections.Generic;
using System.Linq;
using AndcultureCode.CSharp.Core.Models.Entities;
using OpenBreweryDB.Data.Models.Favorites;

namespace OpenBreweryDB.Data.Models.Users
{
    public class User : Entity, IKeyedEntity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string Salt { get; set; }

        public List<Favorite> Favorites { get; set; }

        // Computed Property
        public string FullName => string.Join(" ",
            new string[] { FirstName, LastName }
                .Select(s => s?.Trim())
                .Where(s => !string.IsNullOrEmpty(s)));
    }
}
