import { useBreweriesQuery } from "../queries/autogenerate/hooks";
import { Brewery, PageInfo } from "../queries/autogenerate/schemas";
import { BreweriesQuery } from "../queries/autogenerate/operations";
import { isBreweryNode } from "../validation/isBreweryNode";

const _useBreweriesQuery = () => {
    const { data, loading, error, fetchMore } = useBreweriesQuery();

    const breweries: Brewery[] =
        data?.breweries?.edges
            ?.filter(isBreweryNode)
            ?.map(({ node }) => node as Brewery) ?? [];

    const loadMore = () =>
        fetchMore({
            variables: {
                cursor: data?.breweries?.pageInfo.endCursor,
            },
            updateQuery: (
                previousResult,
                { fetchMoreResult }
            ): BreweriesQuery => {
                const newEdges = fetchMoreResult?.breweries?.edges ?? [];
                const pageInfo = {
                    ...previousResult?.breweries?.pageInfo,
                    ...fetchMoreResult?.breweries?.pageInfo,
                    hasPreviousPage:
                        previousResult?.breweries?.pageInfo.hasPreviousPage,
                    startCursor:
                        previousResult?.breweries?.pageInfo.startCursor,
                };
                const totalCount = fetchMoreResult?.breweries?.totalCount ?? 0;

                if (newEdges.length === 0) {
                    return previousResult;
                }

                return {
                    // Put the new breweries at the end of the list and update `pageInfo`
                    // so we have the new `endCursor` and `hasNextPage` values
                    breweries: {
                        __typename: previousResult?.breweries?.__typename,
                        totalCount,
                        edges: [
                            ...(previousResult?.breweries?.edges ?? []),
                            ...newEdges,
                        ],
                        pageInfo: { ...pageInfo } as PageInfo,
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

export const Services = {
    useBreweriesQuery: _useBreweriesQuery,
};
