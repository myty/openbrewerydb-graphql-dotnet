import { gql } from "apollo-server/dist/exports";

export const typeDefs = gql`
    type Review {
        id: ID!
        breweryId: ID!
        userId: ID!
        title: String!
        body: String!
    }

    input CreateReviewInput {
        clientMutationId: String!
        breweryId: ID!
        userId: ID!
        title: String!
        body: String!
    }

    input DeleteReviewInput {
        clientMutationId: String!
        id: ID!
    }

    type CreateReviewPayload {
        review: Review!
        clientMutationId: String!
    }

    type DeleteReviewPayload {
        deleted: Boolean!
        clientMutationId: String!
    }

    type Query {
        review(id: ID!): Review
        reviews(breweryId: ID): [Review!]!
    }

    type Mutation {
        createReview(input: CreateReviewInput!): CreateReviewPayload!
        deleteReview(input: DeleteReviewInput!): DeleteReviewPayload!
    }
`;
