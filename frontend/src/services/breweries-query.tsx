import { Brewery } from "../types/brewery";
import { useGraphqlQuery } from "../hooks/use-graphql-query";

const BREWERIES_QUERY = `
    query Breweries($cursor: String) {
        breweries(first: 25, after: $cursor) {
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
    data: {
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
    };
}

export const useBreweriesQuery = () => {
    const { data, loading, error, fetchMore } = useGraphqlQuery<BreweriesQuery>(
        BREWERIES_QUERY
    );

    const breweries =
        data?.data?.breweries.edges.map((b: Edge<Brewery>) => b.node) ?? [];

    const loadMore = () =>
        fetchMore(
            {
                cursor: data?.data?.breweries.pageInfo.endCursor,
            },
            (previousResult, fetchMoreResult): BreweriesQuery => {
                const newEdges = fetchMoreResult?.data?.breweries.edges ?? [];
                const pageInfo = {
                    ...previousResult.data?.breweries.pageInfo,
                    ...fetchMoreResult?.data?.breweries.pageInfo,
                    hasPreviousPage:
                        previousResult.data?.breweries.pageInfo.hasPreviousPage,
                    startCursor:
                        previousResult.data?.breweries.pageInfo.startCursor,
                };
                const totalCount =
                    fetchMoreResult?.data?.breweries.totalCount ?? 0;

                if (newEdges.length === 0) {
                    return previousResult;
                }

                return {
                    data: {
                        // Put the new breweries at the end of the list and update `pageInfo`
                        // so we have the new `endCursor` and `hasNextPage` values
                        breweries: {
                            __typename:
                                previousResult.data?.breweries.__typename,
                            totalCount,
                            edges: [
                                ...previousResult.data?.breweries.edges,
                                ...newEdges,
                            ],
                            pageInfo,
                        },
                    },
                };
            }
        );

    const hasMore = data?.data?.breweries?.pageInfo?.hasNextPage ?? false;

    return {
        breweries,
        error,
        hasMore,
        loading,
        loadMore,
    };
};
