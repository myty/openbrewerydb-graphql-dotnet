import { Brewery, BreweryEdge } from "../graphql/autogenerate/schemas";
import { useNearbyBreweriesQuery as useQuery } from "../graphql/autogenerate/hooks";
import { useCallback, useMemo, useState } from "react";
import { CombinedError } from "urql";

interface UseNearbyBreweriesHookResult {
    breweries: Brewery[];
    error?: CombinedError;
    hasMore: boolean;
    loading: boolean;
    loadMore: () => void;
}

type BreweryValueEdge = Required<BreweryEdge> & { node: Brewery };

interface UseNearbyBreweriesOptions {
    latitude?: number;
    longitude?: number;
}

export const useNearbyBreweries = (
    options?: UseNearbyBreweriesOptions,
): UseNearbyBreweriesHookResult => {
    const { latitude, longitude } = options ?? {};
    const [cursor, setCursor] = useState<string>();

    const [result] = useQuery({
        variables: {
            cursor,
            latitude: latitude ?? 0,
            longitude: longitude ?? 0,
        },
        pause: latitude == null || longitude == null,
    });

    const endCursor = useMemo<string | undefined | null>(
        () => result.data?.breweries?.pageInfo.endCursor,
        [result],
    );

    const breweries: Brewery[] =
        result.data?.breweries?.edges
            ?.filter((b): b is BreweryValueEdge => b?.node != null)
            ?.map((edge) => edge.node) ?? [];

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
