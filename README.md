# OpenBreweryDB - GraphQL (.NET Core)

This project started off as an exercise in learning GraphQL in .NET Core by taking an API like, https://www.openbrewerydb.org/ and transforming that into a GraphQL schema.  I initially started with [graphql-dotnet](https://github.com/graphql-dotnet/graphql-dotnet) as the backend server, but have since moved everything over to use [Hot Chocolate](https://hotchocolate.io/). In it's current state I am attempting to have it follow as close as possible to GraphQL schema standards such as the [GraphQL Global Object Identification Specification](https://relay.dev/graphql/objectidentification.htm) that came out of the [Relay](https://relay.dev/) team at Facebook.

I've since added a frontend to make exploring the brewery data.

Some things I am planning to add:

- Pull in Google reviews api data if any is available for breweries (Schema Stitching)
- Nearby breweries (DataLoader)

## Prerequisites

- .NET Core 3.1
- Yarn/NPM

## Getting Started

Run this from your favorite command-line (The first time this runs, it will download seed data so it may take a few minutes more to get things spun up):

    yarn dotnet

Once you see `Application started. Press Ctrl+C to shut down.`, go to this url, https://localhost:5001/graphql/playground/ and play around.

If you'd like to run a working frontend React sample application, in a separate terminal, you will need to do a few things:

1. You will need to get your own Google Maps API key. Here are instructions to do so: https://developers.google.com/maps/documentation/javascript/get-api-key

2. Once you have the key, you'll need to create a local environment file in the `frontend` folder.  Name it `.env.local` and update the contents of the file to be something like this, but replacing the `xxxxxxxxx-xxxxxxxxx-xxxxxxxxx` with the actual API key:

        REACT_APP_GOOGLE_MAPS_API_KEY=xxxxxxxxx-xxxxxxxxx-xxxxxxxxx

3. Finally, before you can run the application, you'll need to install your npm packages

        yarn install

4. Once that is all finished you should be good to go, run:

        yarn frontend
