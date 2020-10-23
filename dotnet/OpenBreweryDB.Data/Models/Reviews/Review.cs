using System;
using AndcultureCode.CSharp.Core.Models.Entities;

namespace OpenBreweryDB.Data.Models.Reviews
{
    public class Review : Entity, IKeyedEntity
    {
        public new long Id { get; set; }

        public long BreweryId { get; set; }

        public Brewery Brewery { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
    }
}
