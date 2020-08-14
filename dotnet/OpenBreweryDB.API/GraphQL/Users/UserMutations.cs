using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using AndcultureCode.CSharp.Core.Extensions;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Types;
using Microsoft.IdentityModel.Tokens;
using OpenBreweryDB.API.GraphQL.Users.Types;
using OpenBreweryDB.Core.Conductors.Users.Interfaces;
using OpenBreweryDB.Data.Models.Users;

namespace OpenBreweryDB.API.GraphQL.Users
{
    [ExtendObjectType(Name = "Mutation")]
    public class UserMutations
    {
        private const string EMAIL_EMPTY = "The email cannot be empty.";
        private const string PASSWORD_EMPTY = "The password cannot be empty.";

        public CreateUserPayload CreateUser(
            CreateUserInput input,
            [Service] IUserConductor userConductor,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(input.Email))
            {
                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage(EMAIL_EMPTY)
                        .SetCode(nameof(EMAIL_EMPTY))
                        .Build());
            }

            if (string.IsNullOrEmpty(input.Password))
            {
                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage(PASSWORD_EMPTY)
                        .SetCode(nameof(PASSWORD_EMPTY))
                        .Build());
            }

            var userResult = userConductor.Create(new User
            {
                FirstName = input.FirstName,
                LastName = input.LastName,
                Email = input.Email,
            }, input.Password);

            if (userResult.HasErrorsOrResultIsNull())
            {
                var error = userResult.Errors.First();

                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage(error.Message)
                        .SetCode(error.Key)
                        .Build());
            }

            return new CreateUserPayload(userResult.ResultObject, input.ClientMutationId);
        }

        public LoginPayload Login(
            LoginInput input,
            [Service] IUserConductor userConductor,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(input.Email))
            {
                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage(EMAIL_EMPTY)
                        .SetCode(nameof(EMAIL_EMPTY))
                        .Build());
            }

            if (string.IsNullOrEmpty(input.Password))
            {
                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage(PASSWORD_EMPTY)
                        .SetCode(nameof(PASSWORD_EMPTY))
                        .Build());
            }

            var userResult = userConductor.FindByEmailAndPassword(input.Email, input.Password);

            if (userResult.HasErrorsOrResultIsNull())
            {
                var error = userResult.Errors.First();

                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage(error.Message)
                        .SetCode(error.Key)
                        .Build());
            }

            var user = userResult.ResultObject;

            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(WellKnownClaimTypes.UserId, user.Id.ToString()),
            });

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.UtcNow.AddHours(12),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Startup.SharedSecret),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return new LoginPayload(user, tokenString, "bearer", input.ClientMutationId);
        }
    }
}
