import { Brewery, BreweryEdge } from "../graphql/autogenerate/schemas";
import { useNearbyBreweriesQuery as useQuery } from "../graphql/autogenerate/hooks";
import { useCallback, useEffect, useMemo, useState } from "react";
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
    const [breweries, setBreweries] = useState<Array<Brewery>>([]);

    const [{ data, error, fetching: loading }] = useQuery({
        variables: {
            cursor,
            latitude: latitude ?? 0,
            longitude: longitude ?? 0,
        },
        pause: latitude == null || longitude == null,
    });

    const endCursor = useMemo<string | undefined | null>(
        () => data?.breweries?.pageInfo.endCursor,
        [data],
    );

    const loadMore = useCallback(() => {
        if (endCursor != null) {
            setCursor(endCursor);
        }
    }, [endCursor]);

    const hasMore = useMemo(() => data?.breweries?.pageInfo?.hasNextPage ?? false, [
        data,
    ]);

    useEffect(() => {
        const nextBreweries: Array<Brewery> =
            data?.breweries?.edges
                ?.filter((b): b is BreweryValueEdge => b?.node != null)
                ?.map((edge) => edge.node) ?? [];

        setBreweries(
            (prev): Array<Brewery> => {
                return [...prev, ...nextBreweries.slice(prev.length)];
            },
        );
    }, [data]);

    return {
        breweries,
        error,
        hasMore,
        loading,
        loadMore,
    };
};
