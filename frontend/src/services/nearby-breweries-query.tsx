import { isBreweryNode } from "../validation/isBreweryNode";
import { Brewery } from "../graphql/autogenerate/schemas";
import { useNearbyBreweriesQuery as _useNearbyBreweriesQuery } from "../graphql/autogenerate/hooks";
import { Utils } from "./breweries-query-helper-utils";

export const useNearbyBreweriesQuery = (
    latitude: number,
    longitude: number
) => {
    const { data, loading, error, fetchMore } = _useNearbyBreweriesQuery({
        variables: {
            latitude,
            longitude,
        },
    });

    const breweries: Brewery[] =
        data?.breweries?.edges
            ?.filter(isBreweryNode)
            ?.map(({ node }) => node as Brewery) ?? [];

    const loadMore = () =>
        fetchMore({
            variables: {
                cursor: data?.breweries?.pageInfo.endCursor,
                latitude,
                longitude,
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
