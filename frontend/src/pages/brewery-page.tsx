import React from "react";
import { RouteComponentProps } from "@reach/router";

interface BreweryPageProps extends RouteComponentProps {
    breweryId?: string;
}

export const BreweryPage = (props: BreweryPageProps) => (
    <div>
        <h2>Brewery: {props.breweryId}</h2>
    </div>
);
