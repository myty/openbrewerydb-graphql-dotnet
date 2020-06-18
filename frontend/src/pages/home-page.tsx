import React, { PropsWithChildren } from "react";
import { RouteComponentProps } from "@reach/router";
import { useQuery } from "@apollo/react-hooks";
import { gql } from "apollo-boost";
import { Text, Heading } from "@chakra-ui/core";

const BREWERIES = gql`
    {
        brewery(id: 1) {
            id
            name
            city
            state
        }
    }
`;

export const HomePage = (props: PropsWithChildren<RouteComponentProps>) => {
    const { loading, error, data } = useQuery(BREWERIES);

    if (loading) return <p>Loading...</p>;
    if (error) return <p>Error :(</p>;

    return (
        <React.Fragment>
            <Heading as="h1" size="2xl" letterSpacing={"-.1rem"}>
                {data.brewery.name}
            </Heading>
            <Text>{`${data.brewery.city}, ${data.brewery.state}`}</Text>
        </React.Fragment>
    );
};
