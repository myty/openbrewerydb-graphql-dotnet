using System;
using System.Collections.Generic;

namespace OpenBreweryDB.API.GraphQL.Common
{
    public class PayloadBase : ClientMutationBase
    {
        protected PayloadBase(string clientMutationId) : base(clientMutationId)
        {
            Errors = Array.Empty<UserError>();
        }

        protected PayloadBase(IReadOnlyList<UserError> errors, string clientMutationId) : base(clientMutationId)
        {
            Errors = errors;
        }

        public IReadOnlyList<UserError> Errors { get; }
    }
}
