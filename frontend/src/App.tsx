import React from "react";
import Header from "./components/header";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import { HomePage } from "./pages/home-page";
import { BreweryPage } from "./pages/brewery-page";
import "./tailwind.output.css";

function App() {
    return (
        <Router>
            <Header title="OpenBreweryDB" />
            <div id="main" style={{ padding: 24 }}>
                <Routes>
                    <Route path="/">
                        <HomePage />
                    </Route>
                    <Route path="breweries/:brewery_id">
                        <BreweryPage />
                    </Route>
                </Routes>
            </div>
        </Router>
    );
}

export default App;
