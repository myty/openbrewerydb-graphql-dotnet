import { isBreweryNode } from "../validation/isBreweryNode";
import { Brewery } from "../graphql/autogenerate/schemas";
import { useBreweriesQuery as _useBreweriesQuery } from "../graphql/autogenerate/hooks";
import { Utils } from "./breweries-query-helper-utils";

export const useBreweriesQuery = () => {
    const { data, loading, error, fetchMore } = _useBreweriesQuery();

    const breweries: Brewery[] =
        data?.breweries?.edges
            ?.filter(isBreweryNode)
            ?.map((edge) => edge?.node as Brewery) ?? [];

    const loadMore = () =>
        fetchMore({
            variables: {
                cursor: data?.breweries?.pageInfo.endCursor,
            },
            updateQuery: Utils.fetchMoreBreweriesUpdateQuery,
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
