import { Application } from "./deps.ts";
import { applyGraphQL, gql } from "./deps.ts";

const app = new Application();

const types = gql`
    type Dino {
        name: String
        image: String
    }
    input DinoInput {
        name: String
        image: String
    }
    type ResolveType {
        done: Boolean
    }
    type Query {
        getDino(name: String): Dino
        getDinos: [Dino!]!
    }
    type Mutation {
        addDino(input: DinoInput!): ResolveType!
    }
`;

const dinos = [
    {
        name: "Tyrannosaurus Rex",
        image: "🦖",
    },
];

const resolvers = {
    Query: {
        getDino: (_, { name }) => {
            const dino = dinos.find((dino) => dino.name.includes(name));
            if (!dino) {
                throw new Error(`No dino name includes ${name}`);
            }
            return dino;
        },
        getDinos: () => {
            return dinos;
        },
    },
    Mutation: {
        addDino: (_, { input: { name, image } }) => {
            dinos.push({
                name,
                image,
            });
            return {
                done: true,
            };
        },
    },
};

const GraphQLService = applyGraphQL({
    typeDefs: types,
    resolvers: resolvers,
});

app.use(GraphQLService.routes(), GraphQLService.allowedMethods());

console.log("Server start at http://localhost:1993");
await app.listen({ port: 1993 });
