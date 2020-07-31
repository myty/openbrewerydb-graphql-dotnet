import React from "react";
import { Brewery } from "../types/brewery";
import { Loading } from "../components/loading";
import { BreweryMap } from "../components/map";
import { HeadingOne } from "../components/heading-1";
import { useParams } from "react-router-dom";
import { useGraphqlQuery } from "../hooks/use-graphql-query";

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
    data: {
        breweries: {
            edges: Array<Edge<Brewery>>;
        };
    };
}

export const BreweryPage = () => {
    const { brewery_id } = useParams();

    const { loading, error, data } = useGraphqlQuery<BreweryQuery>(
        BREWERY_QUERY,
        { brewery_id }
    );

    if (loading) return <Loading />;
    if (error) return <p>Error :(</p>;

    const breweries =
        data?.data?.breweries?.edges?.map((b: Edge<Brewery>) => b.node) ?? [];

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
