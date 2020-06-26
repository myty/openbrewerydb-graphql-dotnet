import React from "react";
import { RouteComponentProps } from "@reach/router";
import { Text, Heading } from "@chakra-ui/core";
import { Brewery } from "../types/brewery";
import { Loading } from "../components/loading";
import { useQuery } from "react-query";
import { request } from "graphql-request";

const BREWERY_QUERY = `
    query Brewery($breweryId: ID!) {
        brewery: node(id: $breweryId) {
            id
            ... on Brewery {
                id
                name
                city
                state
            }
        }
    }
`;

interface BreweryQuery {
    brewery: Brewery;
}

interface BreweryPageProps extends RouteComponentProps {
    breweryId?: string;
}

export const BreweryPage = (props: BreweryPageProps) => {
    const { breweryId } = props;

    const { data, status } = useQuery<BreweryQuery, [string, string?]>(
        ["brewery", breweryId],
        async (key, breweryId) => {
            const results = await request(
                "https://localhost:5001/graphql",
                BREWERY_QUERY,
                { breweryId }
            );

            return results;
        }
    );

    if (status === "loading") return <Loading />;
    if (status === "error") return <p>Error :(</p>;

    return (
        <React.Fragment>
            <Heading as="h1" size="2xl" letterSpacing={"-.1rem"}>
                {data?.brewery.name}
            </Heading>
            <Text>{`${data?.brewery.city}, ${data?.brewery.state}`}</Text>
        </React.Fragment>
    );
};
