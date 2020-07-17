import React from "react";
import { Brewery } from "../types/brewery";
import { Loading } from "../components/loading";
import { useQuery } from "react-query";
import { request } from "graphql-request";
import { BreweryMap } from "../components/map";
import { HeadingOne } from "../components/heading-1";
import { useParams } from "react-router-dom";

const BREWERY_QUERY = `
    query Brewery($brewery_id: String!) {
        breweries(first: 1, brewery_id: $brewery_id) {
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

    const { data, status } = useQuery<BreweryQuery, [string, string?]>(
        ["brewery", brewery_id],
        async (key, brewery_id) => {
            const results = await request(
                "https://localhost:5001/graphql",
                BREWERY_QUERY,
                { brewery_id }
            );

            return results;
        }
    );

    if (status === "loading") return <Loading />;
    if (status === "error") return <p>Error :(</p>;

    const breweries =
        data?.breweries?.edges?.map((b: Edge<Brewery>) => b.node) ?? [];

    if (breweries.length === 0) {
        return (
            <h1 className="text-2xl text-blue-700 leading-tight">
                Brewery Not Found
            </h1>
        );
    }

    const brewery = breweries[0];

    return (
        <React.Fragment>
            <HeadingOne>{brewery.name}</HeadingOne>
            <p className="text-base text-gray-700 leading-normal">{`${brewery.city}, ${brewery.state}`}</p>
            <BreweryMap
                lng={brewery.longitude}
                lat={brewery.latitude}
                text={brewery.name}
            />
        </React.Fragment>
    );
};
