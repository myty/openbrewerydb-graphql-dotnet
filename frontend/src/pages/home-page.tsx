import React, { PropsWithChildren } from "react";
import { RouteComponentProps, navigate } from "@reach/router";
import { Heading, Box, Badge } from "@chakra-ui/core";
import { Brewery } from "../types/brewery";
import { Loading } from "../components/loading";
import { useQuery } from "react-query";
import { request } from "graphql-request";

const BREWERIES_QUERY = `
    query Breweries {
        breweries(first: 25, state: "Pennsylvania", sort: ["city"]) {
            totalCount
            pageInfo {
                startCursor
                hasNextPage
                hasPreviousPage
                endCursor
            }
            edges {
                cursor
                node {
                    name
                    id
                    street
                    city
                    state
                    country
                    website_url
                    brewery_type
                    tag_list
                    phone
                    latitude
                    longitude
                }
            }
        }
    }
`;

interface Edge<T> {
    cursor: string;
    node: T;
}

interface BreweriesQuery {
    breweries: {
        totalCount: number;
        pageInfo: {
            startCursor: string;
            hasNextPage: boolean;
            hasPreviousPage: boolean;
            endCursor: string;
        };
        edges: Array<Edge<Brewery>>;
    };
}

export const HomePage = (props: PropsWithChildren<RouteComponentProps>) => {
    const { data, status } = useQuery<BreweriesQuery, string>(
        "breweries",
        async (key) => {
            const results = await request(
                "https://localhost:5001/graphql",
                BREWERIES_QUERY
            );

            return results;
        }
    );

    if (status === "loading") return <Loading />;
    if (status === "error") return <p>Error :(</p>;

    return (
        <>
            {data?.breweries.edges.map((b: Edge<Brewery>) => (
                <Box
                    as="button"
                    p={5}
                    m={5}
                    w="400px"
                    shadow="md"
                    borderWidth="1px"
                    rounded="md"
                    onClick={() => navigate(`/breweries/${b.node.id}`)}
                    textAlign="left">
                    <Heading fontSize="xl" isTruncated pb={2}>
                        {b.node.name}
                    </Heading>
                    <Box d="flex" alignItems="baseline">
                        <Badge rounded="full" px="2" variantColor="teal">
                            {b.node.brewery_type}
                        </Badge>
                        <Box
                            color="gray.500"
                            fontWeight="semibold"
                            letterSpacing="wide"
                            fontSize="xs"
                            textTransform="uppercase"
                            ml="2">
                            {b.node.city}, {b.node.state}
                        </Box>
                    </Box>
                </Box>
            ))}
        </>
    );
};
