using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AndcultureCode.CSharp.Core.Extensions;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using OpenBreweryDB.API.Extensions;
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
                throw ErrorBuilder.New()
                    .AsGraphQLException(SUBJECT_EMPTY, nameof(SUBJECT_EMPTY));
            }

            if (string.IsNullOrEmpty(input.Body))
            {
                throw ErrorBuilder.New()
                    .AsGraphQLException(BODY_EMPTY, nameof(BODY_EMPTY));
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
                throw breweryResult.Errors
                    .AsGraphQLException();
            }

            review.Brewery = breweryResult.ResultObject;

            await eventSender.SendAsync(
                nameof(ReviewSubscriptions.OnReviewReceived),
                review,
                cancellationToken);

            return new ReviewPayload(review, input.ClientMutationId);
        }
    }
}
