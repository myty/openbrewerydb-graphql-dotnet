import React from "react";
import Header from "./components/header";
import { theme, ThemeProvider, CSSReset } from "@chakra-ui/core";
import ApolloClient from "apollo-boost";
import { ApolloProvider } from "@apollo/react-hooks";
import { Router } from "@reach/router";
import { HomePage } from "./pages/home-page";
import { BreweryPage } from "./pages/brewery-page";

const client = new ApolloClient({
    uri: "https://localhost:5001/graphql",
});

type Breakpoints = string[] & {
    sm?: string;
    md?: string;
    lg?: string;
    xl?: string;
};

const breakpoints: Breakpoints = ["360px", "768px", "1024px", "1440px"];
breakpoints.sm = breakpoints[0];
breakpoints.md = breakpoints[1];
breakpoints.lg = breakpoints[2];
breakpoints.xl = breakpoints[3];

const newTheme = {
    ...theme,
    breakpoints,
};

function App() {
    return (
        <ApolloProvider client={client}>
            <ThemeProvider theme={newTheme}>
                <CSSReset />
                <Header title="OpenBreweryDB" />
                <div id="main" style={{ padding: 24 }}>
                    <Router>
                        <HomePage path="/" />
                        <BreweryPage path="breweries/:breweryId" />
                    </Router>
                </div>
            </ThemeProvider>
        </ApolloProvider>
    );
}

export default App;
