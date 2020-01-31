# OpenBreweryDB - graphql-dotnet

This is an exercise in learning GraphQL in .Net Core by taking an api like, https://www.openbrewerydb.org/ and transforming that into a GraphQL schema.

Some fun things to possibly add to this:

- Pull in Google reviews api data if any is available for breweries
- Explore [Conventions](https://github.com/graphql-dotnet/conventions#getting-started) and [GraphQL.EntityFramework](https://github.com/SimonCropp/GraphQL.EntityFramework) integration

## Prerequisites

- .Net Core 3.1
- Yarn/NPM

## Getting Started

Setup the database:
```bash
yarn database update
```

Load the data (run this only once, since this will reset the entire data set):
```bash
yarn import-data
```

Start up the API:
```bash
yarn serve
```
