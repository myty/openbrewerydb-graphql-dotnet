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

export const HomePage = (props: RouteComponentProps) => {
    const { loading, error, data } = useQuery(BREWERIES);

    if (loading) return <p>Loading...</p>;
    if (error) return <p>Error :(</p>;

    return (
        <div style={{marginTop: 70}}>
            <h1>{data.brewery.name}</h1>
        </div>
    );
};
