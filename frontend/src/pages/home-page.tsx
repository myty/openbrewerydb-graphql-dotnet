import React, { PropsWithChildren } from "react";
import { RouteComponentProps, navigate } from "@reach/router";
import { useQuery } from "@apollo/react-hooks";
import { gql } from "apollo-boost";
import { Heading, Box, Badge } from "@chakra-ui/core";
import { Brewery } from "../types/brewery";
import { Loading } from "../components/loading";

const BREWERIES = gql`
    query Breweries {
        breweries(limit: 1000, state: "Pennsylvania", sort: ["city"]) {
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
`;

interface BreweriesQuery {
    breweries: Brewery[];
}

export const HomePage = (props: PropsWithChildren<RouteComponentProps>) => {
    const { loading, error, data } = useQuery<BreweriesQuery>(BREWERIES);

    if (loading) return <Loading />;
    if (error || !data?.breweries) return <p>Error :(</p>;

    return (
        <>
            {data.breweries.map((b: Brewery) => (
                <Box
                    as="button"
                    p={5}
                    m={5}
                    w="400px"
                    shadow="md"
                    borderWidth="1px"
                    rounded="md"
                    onClick={() => navigate(`/breweries/${b.id}`)}
                    textAlign="left">
                    <Heading fontSize="xl" isTruncated pb={2}>
                        {b.name}
                    </Heading>
                    <Box d="flex" alignItems="baseline">
                        <Badge rounded="full" px="2" variantColor="teal">
                            {b.brewery_type}
                        </Badge>
                        <Box
                            color="gray.500"
                            fontWeight="semibold"
                            letterSpacing="wide"
                            fontSize="xs"
                            textTransform="uppercase"
                            ml="2">
                            {b.city}, {b.state}
                        </Box>
                    </Box>
                </Box>
            ))}
        </>
    );
};
