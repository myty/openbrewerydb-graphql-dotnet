using OpenBreweryDB.Data.Core;

namespace OpenBreweryDB.Data.Models
{
    public class BreweryTag : BaseEntity
    {
        public long BreweryId { get; set; }
        public Brewery Brewery { get; set; }

        public long TagId { get; set; }
        public Tag Tag { get; set; }
    }
}