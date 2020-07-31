import React from "react";
import { Brewery } from "../types/brewery";
import { Loading } from "../components/loading";
import { BreweryMap } from "../components/map";
import { HeadingOne } from "../components/heading-1";
import { useParams } from "react-router-dom";
import { useQuery, gql } from "@apollo/client";

const BREWERY_QUERY = gql`
    query Brewery($brewery_id: String!) {
        breweries(first: 1, brewery_id: $brewery_id) @connection(key: "brewery", filter: ["brewery_id"]) {
            edges {
                node {
                    id
                    ... on Brewery {
                        id
                        name
                        city
                        state
                        latitude
                        longitude
                    }
                }
            }
        }
    }
`;

interface Edge<T> {
    cursor: string;
    node: T;
}

interface BreweryQuery {
    breweries: {
        edges: Array<Edge<Brewery>>;
    };
}

export const BreweryPage = () => {
    const { brewery_id } = useParams();

    const { loading, error, data } = useQuery(BREWERY_QUERY, {
        variables: { brewery_id }
    });

    if (loading) return <Loading />;
    if (error) return <p>Error :(</p>;

    const breweries =
        data?.breweries?.edges?.map((b: Edge<Brewery>) => b.node) ?? [];

    if (breweries.length === 0) {
        return (
            <h1 className="text-2xl font-semibold leading-tight text-yellow-900">
                Brewery Not Found
            </h1>
        );
    }

    const brewery = breweries[0];

    return (
        <React.Fragment>
            <HeadingOne>{brewery.name}</HeadingOne>
            <p className="text-base font-semibold leading-normal text-yellow-700">{`${brewery.city}, ${brewery.state}`}</p>
            <BreweryMap
                lng={brewery.longitude}
                lat={brewery.latitude}
                text={brewery.name}
            />
        </React.Fragment>
    );
};
