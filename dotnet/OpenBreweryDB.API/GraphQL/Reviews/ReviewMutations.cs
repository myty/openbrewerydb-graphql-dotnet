using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AndcultureCode.CSharp.Core.Extensions;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using OpenBreweryDB.API.GraphQL.Reviews.Types;
using OpenBreweryDB.Core.Conductors.Breweries.Interfaces;
using OpenBreweryDB.Data;
using OpenBreweryDB.Data.Models.Reviews;

namespace OpenBreweryDB.API.GraphQL.Reviews
{
    [ExtendObjectType(Name = "Mutation")]
    public class ReviewMutations
    {
        private const string SUBJECT_EMPTY = "The subject cannot be empty.";
        private const string BODY_EMPTY = "The body cannot be empty.";

        public async Task<ReviewPayload> CreateReview(
            ReviewInput input,
            [Service] BreweryDbContext dbContext,
            [Service] IBreweryConductor breweryConductor,
            [Service] ITopicEventSender eventSender,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(input.Subject))
            {
                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage(SUBJECT_EMPTY)
                        .SetCode(nameof(SUBJECT_EMPTY))
                        .Build());
            }

            if (string.IsNullOrEmpty(input.Body))
            {
                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage(BODY_EMPTY)
                        .SetCode(nameof(BODY_EMPTY))
                        .Build());
            }

            var review = new Review
            {
                Subject = input.Subject,
                Body = input.Body,
                BreweryId = input.BreweryId,
                CreatedOn = DateTimeOffset.Now,
            };

            _ = await dbContext.AddAsync(review);
            _ = await dbContext.SaveChangesAsync(cancellationToken);

            var breweryResult = breweryConductor.Find(input.BreweryId);

            if (breweryResult.HasErrorsOrResultIsNull())
            {
                var error = breweryResult.Errors.First();

                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage(error.Message)
                        .SetCode(error.Key)
                        .Build());
            }

            review.Brewery = breweryResult.ResultObject;

            await eventSender.SendAsync("reviews", review, cancellationToken);

            return new ReviewPayload(review, input.ClientMutationId);
        }
    }
}
