# OpenBreweryDB - GraphQL (.NET Core)

This project is a learning tool and playground for ideas and concepts around GraphQL in the world of .NET Core and React. It was initially an exercise to learn GraphQL by taking an API like, https://www.openbrewerydb.org/ and transforming that into a GraphQL schema. I've experimented with [graphql-dotnet](https://github.com/graphql-dotnet/graphql-dotnet) as well as [Hot Chocolate](https://hotchocolate.io/). Both are well polished frameworks if you want to do GraphQL in .NET. Currently, I am focused on **graphql-dotnet**.

## Prerequisites

-   .NET Core 5
-   Yarn/NPM
-   Docker

## Getting Started

To get started, will need all of the prerequisites installed, then run this from the command line:

    yarn setup

### Server (.NET Core)

This is using the latest bits from the graphql-dotnet/relay project and since that is not released on nuget yet, it is being referenced via a git submodules. Make sure you run this before you start on the dotnet portion of the code:

    git submodule update --init --recursive

To start the GraphQL server, run this (The first time this runs, it will download seed data so it may take a few minutes more to get things spun up):

    yarn db:start

For the time being, wait 10-15 seconds to allow the SQL instance to boot up and get into a ready state. Once SQL is ready, you can then run

    yarn dotnet

Once you see `Application started. Press Ctrl+C to shut down.`, go to this url, https://localhost:5001/api/graphql and play around with the GraphQL data and schema. It is using the Altair UI.

### Client (React, Vite, TailwindCSS)

If you'd like to run a working frontend React sample application, in a separate terminal, you will need to do a few things:

1.  You will need to get your own Google Maps API key. Here are instructions to do so: https://developers.google.com/maps/documentation/javascript/get-api-key

2.  Once you have the key, you'll need to create a local environment file in the `frontend` folder. Name it `.env.local` and update the contents of the file to be something like this, but replacing the `xxxxxxxxx-xxxxxxxxx-xxxxxxxxx` with the actual API key:

        VITE_GOOGLE_MAPS_KEY=xxxxxxxxx-xxxxxxxxx-xxxxxxxxx

3.  Once that is all finished, you should be good to go, run:

        yarn frontend

4.  Navigate to http://localhost:3000/

### Note

When you are all finished and you want to stop your docker SQL instance container, you can run:

    yarn db:stop
