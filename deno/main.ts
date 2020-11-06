import { Application, Router } from "./deps.ts";
import { applyGraphQL, gql } from "./deps.ts";
import type { InputType, PayloadType } from "./types.ts";

const app = new Application();

app.use(async (ctx, next) => {
    await next();
    const rt = ctx.response.headers.get("X-Response-Time");
    console.log(`${ctx.request.method} ${ctx.request.url} - ${rt}`);
});

app.use(async (ctx, next) => {
    const start = Date.now();
    await next();
    const ms = Date.now() - start;
    ctx.response.headers.set("X-Response-Time", `${ms}ms`);
});

const types = gql`
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
    type CreateReviewPayload {
        review: Review!
        clientMutationId: String!
    }
    type Query {
        review(id: ID!): Review
        reviews(breweryId: ID): [Review!]!
    }
    type Mutation {
        createReview(input: CreateReviewInput!): CreateReviewPayload!
    }
`;

type Review = {
    id: number;
    breweryId: number;
    userId: number;
    title: string;
    body: string;
};

const reviews: Review[] = [
    {
        id: 1,
        breweryId: 1,
        userId: 1,
        title: "Test Title",
        body: "Test body content",
    },
];

const resolvers = {
    Query: {
        review: (_: unknown, { id }: Review) => {
            const review = reviews.find((review) => review.id === id);
            if (review == null) {
                throw new Error(
                    `Review with the id of '${id}' was not able to be found.`
                );
            }
            return review;
        },
        reviews: (_: unknown, { breweryId }: Review) => {
            if (breweryId == null) {
                return reviews;
            }

            return (
                reviews.filter((review) => review.breweryId === breweryId) ?? []
            );
        },
    },
    Mutation: {
        createReview: (
            _: unknown,
            {
                input: { clientMutationId, breweryId, title, body, userId },
            }: InputType<Review>
        ): PayloadType<{ review: Review }> => {
            const id =
                reviews.reduce((prev, current) => {
                    if (prev > current.id) {
                        return prev;
                    }

                    return current.id;
                }, 0) + 1;

            const review: Review = {
                id,
                breweryId,
                title,
                body,
                userId,
            };

            reviews.push(review);

            return {
                review,
                clientMutationId,
            };
        },
    },
};

const GraphQLService = await applyGraphQL<Router>({
    Router,
    typeDefs: types,
    resolvers: resolvers,
});

app.use(GraphQLService.routes(), GraphQLService.allowedMethods());

console.log("Server start at http://localhost:1993");
await app.listen({ port: 1993 });
