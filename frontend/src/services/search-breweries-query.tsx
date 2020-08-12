import { isBreweryNode } from "../validation/isBreweryNode";
import { Brewery } from "../graphql/autogenerate/schemas";
import { useSearchQuery as _useSearchQuery } from "../graphql/autogenerate/hooks";
import { Utils } from "./breweries-query-helper-utils";

export const useSearchQuery = (search: string) => {
    const { data, loading, error, fetchMore } = _useSearchQuery({
        variables: { search },
    });

    const breweries: Brewery[] =
        data?.breweries?.edges
            ?.filter(isBreweryNode)
            ?.map(({ node }) => node as Brewery) ?? [];

    const loadMore = () =>
        fetchMore({
            variables: {
                cursor: data?.breweries?.pageInfo.endCursor,
                search,
            },
            updateQuery: Utils.fetchMoreBreweriesUpdateQuery,
        });

    const hasMore = data?.breweries?.pageInfo?.hasNextPage ?? false;

    const totalResults = data?.breweries?.totalCount ?? 0;

    return {
        breweries,
        error,
        hasMore,
        loading,
        loadMore,
        totalResults
    };
};
