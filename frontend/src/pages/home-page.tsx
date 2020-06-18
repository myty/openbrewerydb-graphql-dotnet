import React from "react";
import { RouteComponentProps } from "@reach/router";
import { useQuery } from "@apollo/react-hooks";
import { gql } from "apollo-boost";

const BREWERIES = gql`
    {
        brewery(id: 1) {
            id
            name
        }
    }
`;

type Brewery = {
    id: string;
    name: string;
};

export const HomePage = (props: RouteComponentProps) => {
    const { loading, error, data } = useQuery<Brewery>(BREWERIES);

    if (loading) return <p>Loading...</p>;
    if (error || !data) return <p>Error :(</p>;

    return <h1>{data.name}</h1>;
};
