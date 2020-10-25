
using System.Collections.Generic;
using System.Linq;
using AndcultureCode.CSharp.Core.Interfaces;

namespace OpenBreweryDB.API.Extensions
{
    public static class ErrorExtensions
    {
        public static HotChocolate.GraphQLException AsGraphQLException(this IEnumerable<IError> errors)
        {
            return new HotChocolate.GraphQLException(
                errors.Select(error =>
                    HotChocolate.ErrorBuilder.New()
                        .SetMessage(error.Message)
                        .SetCode(error.Key)
                        .Build()));
        }

        public static HotChocolate.GraphQLException AsGraphQLException(this IError error)
        {
            return new HotChocolate.GraphQLException(
                HotChocolate.ErrorBuilder.New()
                    .SetMessage(error.Message)
                    .SetCode(error.Key)
                    .Build());
        }

        public static HotChocolate.GraphQLException AsGraphQLException(
            this HotChocolate.ErrorBuilder errorbuilder,
            string message,
            string code) => new HotChocolate.GraphQLException(
            errorbuilder
                .SetMessage(message)
                .SetCode(code)
                .Build());
    }
}
