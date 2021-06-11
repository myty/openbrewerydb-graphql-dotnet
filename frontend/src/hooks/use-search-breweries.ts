import { Brewery, BreweryEdge } from "../graphql/autogenerate/schemas";
import { useSearchQuery as useQuery } from "../graphql/autogenerate/hooks";
import { useCallback, useMemo, useState } from "react";

type BreweryValueEdge = Required<BreweryEdge> & { node: Brewery };

export const useSearchBreweries = (search: string) => {
    const [cursor, setCursor] = useState<string>();
    const [result] = useQuery({ variables: { search, cursor } });

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

    const totalResults = useMemo(() => result.data?.breweries?.totalCount ?? 0, [result]);

    return {
        breweries,
        error: result.error,
        hasMore,
        loading: result.fetching,
        loadMore,
        totalResults,
    };
};
