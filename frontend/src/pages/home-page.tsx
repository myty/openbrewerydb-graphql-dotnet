import React from "react";
import { Brewery } from "../types/brewery";
import { Loading } from "../components/loading";
import { useQuery } from "react-query";
import { request } from "graphql-request";
import { HeadingOne } from "../components/heading-1";
import { useNavigate } from "react-router-dom";

const BREWERIES_QUERY = `
    query Breweries {
        breweries(first: 100, state: "Pennsylvania", sort: ["city"]) {
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
                    brewery_id
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

export const HomePage = () => {
    const navigate = useNavigate();

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

    const breweries =
        data?.breweries.edges.map((b: Edge<Brewery>) => b.node) ?? [];

    if (breweries.length === 0) {
        return <HeadingOne>There are no breweries to display.</HeadingOne>;
    }

    return (
        <>
            {breweries.map((b: Brewery) => (
                <button
                    key={b.id}
                    className="group bg-blue-200 hover:bg-blue-500 w-64 h-32 m-4"
                    onClick={() => navigate(`/breweries/${b.brewery_id}`)}>
                    <p className="text-gray-900 group-hover:text-white truncate w-full px-3 font-semibold">
                        {b.name}
                    </p>
                    <p className="text-gray-700 group-hover:text-white">
                        {b.city}, {b.state}
                    </p>
                </button>
            ))}
        </>
    );
};
