using System.Collections.Generic;
using OpenBreweryDB.API.Data.Core;

namespace OpenBreweryDB.API.Data.Models
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }

        public List<BreweryTag> BreweryTags { get; set; }
    }
}