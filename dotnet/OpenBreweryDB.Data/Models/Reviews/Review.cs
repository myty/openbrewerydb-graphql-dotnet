using System;
using AndcultureCode.CSharp.Core.Models.Entities;

namespace OpenBreweryDB.Data.Models.Reviews
{
    public class Review : Entity, IKeyedEntity
    {
        public long BreweryId { get; set; }

        public Brewery Brewery { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
    }
}
