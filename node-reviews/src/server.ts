import { ApolloServer }  from "apollo-server";
import { typeDefs } from "./type-defs";
import resolvers from "./resolvers";
import { createConnection } from "typeorm";

async function run() {
    await createConnection({
        type: "postgres",
        host: "reviews-db",
        port: 5432,
        username: "unicorn_user",
        password: "magical_password",
        database: "rainbow_database",
        synchronize: false,
        logging: false,
        entities: ["dist/entity/**/*.js"],
    });

    // The ApolloServer constructor requires two parameters: your schema
    // definition and your set of resolvers.
    const server = new ApolloServer({ typeDefs, resolvers });

    // The `listen` method launches a web server.
    server.listen(1993).then(({ url }) => {
        console.log(`🚀 Server ready at ${url}graphql`);
    });
}

run();
