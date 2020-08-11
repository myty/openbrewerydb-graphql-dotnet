import { BreweriesQuery } from "../graphql/autogenerate/operations";
import { PageInfo } from "../graphql/autogenerate/schemas";

const fetchMoreBreweriesUpdateQuery = (
    previousResult: BreweriesQuery,
    { fetchMoreResult }: { fetchMoreResult?: BreweriesQuery }
): BreweriesQuery => {
    const newEdges = fetchMoreResult?.breweries?.edges ?? [];
    const pageInfo = {
        ...previousResult?.breweries?.pageInfo,
        ...fetchMoreResult?.breweries?.pageInfo,
        hasPreviousPage: previousResult?.breweries?.pageInfo.hasPreviousPage,
        startCursor: previousResult?.breweries?.pageInfo.startCursor,
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
            edges: [...(previousResult?.breweries?.edges ?? []), ...newEdges],
            pageInfo: { ...pageInfo } as PageInfo,
        },
    };
};

export const Utils = {
    fetchMoreBreweriesUpdateQuery,
};
