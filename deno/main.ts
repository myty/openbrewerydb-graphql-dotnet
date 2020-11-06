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
    type Beer {
        breweryId: String!
        id: ID!
        name: String!
        type: String!
    }
    input CreateBeerInput {
        clientMutationId: String!
        name: String!
        type: String!
        breweryId: String!
    }
    type CreateBeerPayload {
        beer: Beer!
        clientMutationId: String!
    }
    type Query {
        beer(id: ID!): Beer
        beers: [Beer!]!
    }
    type Mutation {
        createBeer(input: CreateBeerInput!): CreateBeerPayload!
    }
`;

type Beer = { id: number; breweryId: string; name: string; type: string };

const beers: Beer[] = [
    {
        id: 1,
        breweryId: "costumes-and-karaoke",
        name: "Costumes & Karaoke",
        type: "IPA",
    },
];

const resolvers = {
    Query: {
        beer: (_: unknown, { id }: Beer) => {
            const beer = beers.find((beer) => beer.id === id);
            if (!beer) {
                throw new Error(
                    `Beer with the id of '${id}' was not able to be found.`
                );
            }
            return beer;
        },
        beers: () => {
            return beers;
        },
    },
    Mutation: {
        createBeer: (
            _: unknown,
            {
                input: { clientMutationId, name, type, breweryId },
            }: InputType<Beer>
        ): PayloadType<{ beer: Beer }> => {
            const id =
                beers.reduce((prev, current) => {
                    if (prev > current.id) {
                        return prev;
                    }

                    return current.id;
                }, 0) + 1;

            const beer: Beer = {
                id,
                breweryId,
                name,
                type,
            };

            beers.push(beer);

            return {
                beer,
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
