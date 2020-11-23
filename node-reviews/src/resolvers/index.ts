import { getReview } from "./Query/get-review";
import { getReviews } from "./Query/get-reviews";
import { createReview } from "./Mutation/create-review";
import { deleteReview } from "./Mutation/delete-review";

// Resolvers define the technique for fetching the types defined in the
// schema. This resolver retrieves books from the "books" array above.
export default {
    Query: {
        review: getReview,
        reviews: getReviews,
    },
    Mutation: {
        createReview,
        deleteReview
    }
};
