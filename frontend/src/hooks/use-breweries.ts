import { Brewery, BreweryEdge } from "../graphql/autogenerate/schemas";
import { useBreweriesQuery as useQuery } from "../graphql/autogenerate/hooks";
import { useEffect, useState } from "react";
import { useCallback } from "react";
import { useMemo } from "react";

type BreweryValueEdge = Required<BreweryEdge> & { node: Brewery };

export const useBreweries = () => {
    const [cursor, setCursor] = useState<string>();
    const [breweries, setBreweries] = useState<Array<Brewery>>([]);

    const [{ data, error, fetching: loading }] = useQuery({ variables: { cursor } });

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
