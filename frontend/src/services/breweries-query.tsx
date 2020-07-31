import { Brewery } from "../types/brewery";
import { useQuery, gql } from "@apollo/client";

const BREWERIES_QUERY = gql`
    query Breweries($cursor: String) {
        breweries(first: 25, after: $cursor) @connection(key: "breweries") {
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
        __typename: string;
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

export const useBreweriesQuery = () => {
    const { data, loading, error, fetchMore } = useQuery<BreweriesQuery>(
        BREWERIES_QUERY
    );

    const breweries =
        data?.breweries.edges.map((b: Edge<Brewery>) => b.node) ?? [];

    const loadMore = () =>
        fetchMore({
            variables: {
                cursor: data?.breweries.pageInfo.endCursor,
            },
            updateQuery: (
                previousResult,
                { fetchMoreResult }
            ): BreweriesQuery => {
                const newEdges = fetchMoreResult?.breweries.edges ?? [];
                const pageInfo = {
                    ...previousResult.breweries.pageInfo,
                    ...fetchMoreResult?.breweries.pageInfo,
                    hasPreviousPage: previousResult.breweries.pageInfo.hasPreviousPage,
                    startCursor: previousResult.breweries.pageInfo.startCursor,
                };
                const totalCount = fetchMoreResult?.breweries.totalCount ?? 0;

                if (newEdges.length === 0) {
                    return previousResult;
                }

                return {
                    // Put the new breweries at the end of the list and update `pageInfo`
                    // so we have the new `endCursor` and `hasNextPage` values
                    breweries: {
                        __typename: previousResult.breweries.__typename,
                        totalCount,
                        edges: [...previousResult.breweries.edges, ...newEdges],
                        pageInfo,
                    },
                };
            },
        });

    const hasMore = data?.breweries?.pageInfo?.hasNextPage ?? false;

    return {
        breweries,
        error,
        hasMore,
        loading,
        loadMore,
    };
};
