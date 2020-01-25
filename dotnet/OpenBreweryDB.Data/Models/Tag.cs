using System.Collections.Generic;
using AndcultureCode.CSharp.Core.Models.Entities;

namespace OpenBreweryDB.Data.Models
{
    public class Tag : Entity
    {
        public string Name { get; set; }

        public List<BreweryTag> BreweryTags { get; set; }
    }
}
