import React from "react";
import { Brewery } from "../types/brewery";
import { Loading } from "../components/loading";
import { useQuery } from "react-query";
import { request } from "graphql-request";
import { HeadingOne } from "../components/heading-1";

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
                <a
                    key={b.id}
                    href={`/breweries/${b.brewery_id}`}
                    className="outline-none focus:shadow-outline focus:bg-gray-100 block overflow-hidden border border-gray-300 shadow rounded-md p-4 max-w-xl w-full mx-auto my-4">
                    <div className="flex space-x-4">
                        <div className="rounded-full bg-gray-400 h-12 w-12"></div>
                        <div className="flex-1 space-y-4 py-1">
                            <div className="px-3 w-3/4">
                                <div className="truncate text-md w-full font-bold tracking-tighter block">
                                    {b.name}
                                </div>
                                <div className="text-opacity-50 subpixel-antialiased text-xs uppercase block">
                                    {b.city}, {b.state}
                                </div>
                            </div>
                        </div>
                    </div>
                </a>
            ))}
        </>
    );
};
