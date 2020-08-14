using System;
using AndcultureCode.CSharp.Core.Models.Entities;
using HotChocolate;
using OpenBreweryDB.Data.Models.Users;

namespace OpenBreweryDB.Data.Models.Reviews
{
    public class Review : Entity, IKeyedEntity
    {
        [GraphQLIgnore]
        public long BreweryId { get; set; }

        [GraphQLNonNullType]
        public Brewery Brewery { get; set; }

        [GraphQLNonNullType]
        public string Subject { get; set; }

        [GraphQLNonNullType]
        public string Body { get; set; }

        [GraphQLNonNullType]
        public DateTimeOffset CreatedOn { get; set; }
    }
}
