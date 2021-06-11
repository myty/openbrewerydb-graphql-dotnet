import React from "react";
import Header from "./components/header";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import { HomePage } from "./pages/home-page";
import { BreweryPage } from "./pages/brewery-page";
import { NearbyPage } from "./pages/nearby-page";
import { SearchPage } from "./pages/search-page";
import { createClient, dedupExchange, fetchExchange, Provider } from "urql";
import { cacheExchange } from "@urql/exchange-graphcache";
import { relayPagination } from "@urql/exchange-graphcache/extras";

const httpLink = "https://localhost:5001/api/graphql";
const wsLink = "wss://localhost:5001/api/graphql";

const client = createClient({
    exchanges: [
        dedupExchange,
        cacheExchange({
            resolvers: {
                Query: {
                    breweries: relayPagination(),
                    nearbyBreweries: relayPagination(),
                },
            },
        }),
        fetchExchange,
    ],
    url: httpLink,
});

function App() {
    return (
        <Provider value={client}>
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
        </Provider>
    );
}

export default App;
