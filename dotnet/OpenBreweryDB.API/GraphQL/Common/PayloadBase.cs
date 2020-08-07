using System;
using System.Collections.Generic;
using HotChocolate;

namespace OpenBreweryDB.API.GraphQL.Common
{
    public class PayloadBase
    {
        protected PayloadBase(string clientMutationId)
        {
            Errors = Array.Empty<UserError>();
            ClientMutationId = clientMutationId;
        }

        protected PayloadBase(IReadOnlyList<UserError> errors, string clientMutationId)
        {
            Errors = errors;
            ClientMutationId = clientMutationId;
        }

        public IReadOnlyList<UserError> Errors { get; }

        [GraphQLDescription("Relay Client Mutation Id")]
        [GraphQLNonNullType]
        public string ClientMutationId { get; }
    }
}
