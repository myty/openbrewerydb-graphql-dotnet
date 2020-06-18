import React from "react";
import { RouteComponentProps } from "@reach/router";
import { useQuery } from "@apollo/react-hooks";
import { gql } from "apollo-boost";
import { Text, Heading } from "@chakra-ui/core";

const BREWERIES = gql`
    query Brewery($breweryId: ID!) {
        brewery(id: $breweryId) {
            id
            name
            city
            state
        }
    }
`;

interface BreweryQuery {
    brewery: {
        id: number;
        name: string;
        city: string;
        state: string;
    };
}

interface BreweryPageProps extends RouteComponentProps {
    breweryId?: string;
}

export const BreweryPage = (props: BreweryPageProps) => {
    const { breweryId } = props;
    const { loading, error, data } = useQuery<BreweryQuery>(BREWERIES, {
        variables: { breweryId },
    });

    if (loading) return <p>Loading...</p>;
    if (error || !data?.brewery) return <p>Error :(</p>;

    return (
        <React.Fragment>
            <Heading as="h1" size="2xl" letterSpacing={"-.1rem"}>
                {data.brewery.name}
            </Heading>
            <Text>{`${data.brewery.city}, ${data.brewery.state}`}</Text>
        </React.Fragment>
    );
};
