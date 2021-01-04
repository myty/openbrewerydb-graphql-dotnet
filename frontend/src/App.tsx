import React from "react";
import Header from "./components/header";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import { HomePage } from "./pages/home-page";
import { BreweryPage } from "./pages/brewery-page";
import { ApolloClient, InMemoryCache } from "@apollo/client";
import { ApolloProvider } from "@apollo/client";
import { NearbyPage } from "./pages/nearby-page";
import "./tailwind.output.css";
import { SearchPage } from "./pages/search-page";
import { split, HttpLink } from "@apollo/client";
import { getMainDefinition } from "@apollo/client/utilities";
import { WebSocketLink } from "@apollo/client/link/ws";

const httpLink = new HttpLink({
    uri: "https://localhost:5001/api/graphql",
});

const wsLink = new WebSocketLink({
    uri: `wss://localhost:5001/api/graphql`,
    options: {
        reconnect: true,
    },
});

// The split function takes three parameters:
//
// * A function that's called for each operation to execute
// * The Link to use for an operation if the function returns a "truthy" value
// * The Link to use for an operation if the function returns a "falsy" value
const link = split(
    ({ query }) => {
        const definition = getMainDefinition(query);
        return (
            definition.kind === "OperationDefinition" &&
            definition.operation === "subscription"
        );
    },
    wsLink,
    httpLink
);

const client = new ApolloClient({
    link,
    cache: new InMemoryCache(),
});

function App() {
    return (
        <ApolloProvider client={client}>
            <Router>
                <Header title="OpenBreweryDB" />
                <div id="main" style={{ padding: 24 }}>
                    <Routes>
                        <Route path="/">
                            <HomePage />
                        </Route>
                        <Route path="nearby">
                            <NearbyPage />
                        </Route>
                        {/* <Route path="reviews">
                            <ReviewPage />
                        </Route> */}
                        <Route path="search">
                            <SearchPage />
                        </Route>
                        <Route path="breweries/:external_id">
                            <BreweryPage />
                        </Route>
                    </Routes>
                </div>
            </Router>
        </ApolloProvider>
    );
}

export default App;
