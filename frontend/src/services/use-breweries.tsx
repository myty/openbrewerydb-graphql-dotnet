import { Brewery, BreweryEdge } from "../graphql/autogenerate/schemas";
import { useBreweriesQuery as useQuery } from "../graphql/autogenerate/hooks";
import { useState } from "react";
import { useCallback } from "react";
import { useMemo } from "react";

type BreweryValueEdge = Required<BreweryEdge> & { node: Brewery };

export const useBreweries = () => {
    const [cursor, setCursor] = useState<string>();
    const [result] = useQuery({ variables: { cursor } });

    const breweries: Brewery[] =
        result.data?.breweries?.edges
            ?.filter((b): b is BreweryValueEdge => b?.node != null)
            ?.map((edge) => edge.node) ?? [];

    const endCursor = useMemo<string | undefined | null>(
        () => result.data?.breweries?.pageInfo.endCursor,
        [result],
    );

    const loadMore = useCallback(() => {
        if (endCursor != null) {
            setCursor(endCursor);
        }
    }, [endCursor]);

    const hasMore = useMemo(
        () => result.data?.breweries?.pageInfo?.hasNextPage ?? false,
        [result],
    );

    return {
        breweries,
        error: result.error,
        hasMore,
        loading: result.fetching,
        loadMore,
    };
};
