import React from "react";
import Header from "./components/header";
import { theme, ThemeProvider, CSSReset } from "@chakra-ui/core";
import { Router } from "@reach/router";
import { HomePage } from "./pages/home-page";
import { BreweryPage } from "./pages/brewery-page";

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
    );
}

export default App;
