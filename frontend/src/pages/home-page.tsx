import React, { useState } from "react";
import { Brewery } from "../types/brewery";
import { Loading } from "../components/loading";
import { useQuery } from "react-query";
import { request } from "graphql-request";
import { HeadingOne } from "../components/heading-1";
import InfiniteScroll from 'react-infinite-scroller';

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
    const [ hasMore, setHasMore ] = useState<boolean>();
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

    const loadMore = (_page: number) => {

    };

    return (
        <InfiniteScroll
            loadMore={loadMore}
            hasMore={hasMore}
            loader={<div className="loader" key={0}>Loading ...</div>}
>
            {breweries.map((b: Brewery) => (
                <a
                    key={b.id}
                    href={`/breweries/${b.brewery_id}`}
                    className="block w-full max-w-xl p-4 mx-auto my-4 overflow-hidden border border-gray-300 rounded-md shadow outline-none focus:shadow-outline focus:bg-gray-100">
                    <div className="flex space-x-4">
                        <div className="w-12 h-12 bg-gray-400 rounded-full"></div>
                        <div className="flex-1 py-1 space-y-4">
                            <div className="w-3/4 px-3">
                                <div className="block w-full font-bold tracking-tighter truncate text-md">
                                    {b.name}
                                </div>
                                <div className="block text-xs subpixel-antialiased text-opacity-50 uppercase">
                                    {b.city}, {b.state}
                                </div>
                            </div>
                        </div>
                    </div>
                </a>
            ))}
            </InfiniteScroll>
    );
};
