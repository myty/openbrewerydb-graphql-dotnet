import { Application, Router } from "./deps.ts";
import { applyGraphQL, gql } from "./deps.ts";
import Review, { IReview } from "./models/review.ts";
import type { InputType, PayloadType } from "./types.ts";
import { db } from "./db.ts";

db.link([Review]);
await db.sync();

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

const resolvers = {
    Query: {
        review: async (_: unknown, { id }: IReview) => {
            const review = await Review.find(id);
            if (review == null) {
                throw new Error(
                    `Review with the id of '${id}' was not able to be found.`
                );
            }
            return review;
        },
        reviews: async (
            _: unknown,
            { breweryId }: IReview
        ): Promise<IReview[]> => {
            if (breweryId == null) {
                return await Review.all();
            }

            return await Review.where("breweryId", breweryId).get();
        },
    },
    Mutation: {
        createReview: async (
            _: unknown,
            {
                input: { clientMutationId, breweryId, title, body, userId },
            }: InputType<IReview>
        ): Promise<PayloadType<{ review: IReview }>> => {
            const review: IReview = await Review.create({
                body,
                breweryId,
                title,
                userId,
            });

            return {
                review: review,
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
