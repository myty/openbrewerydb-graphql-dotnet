using System.ComponentModel.DataAnnotations;

namespace OpenBreweryDB.Data.Models
{
    public interface IKeyedEntity
    {
        [Key]
        long Id { get; set; }
    }
}
