import { Database } from "./deps.ts";
import Review from "./models/review.ts";

export const db = new Database("postgres", {
    database: "mrainbow_database",
    host: "database",
    username: "unicorn_user",
    password: "magical_password",
    port: 5432, // optional
});

db.link([Review]);
db.sync();
