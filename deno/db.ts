import { Database } from "./deps.ts";

export const db = new Database("postgres", {
    database: "rainbow_database",
    host: "database",
    username: "unicorn_user",
    password: "magical_password",
    port: 5432, // optional
});
