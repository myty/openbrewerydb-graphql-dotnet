using System.Collections.Generic;
using AndcultureCode.CSharp.Core.Models.Entities;

namespace OpenBreweryDB.Data.Models
{
    public class Tag : Entity, IKeyedEntity
    {
        public new long Id { get; set; }

        public string Name { get; set; }

        public List<BreweryTag> BreweryTags { get; set; }
    }
}
