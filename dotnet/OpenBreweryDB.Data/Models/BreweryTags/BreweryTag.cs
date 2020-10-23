using AndcultureCode.CSharp.Core.Models.Entities;

namespace OpenBreweryDB.Data.Models
{
    public class BreweryTag : Entity, IKeyedEntity
    {
        public new long Id { get; set; }
        public long BreweryId { get; set; }
        public Brewery Brewery { get; set; }

        public long TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
