import { isBreweryNode } from "../validation/isBreweryNode";
import { Brewery } from "../graphql/autogenerate/schemas";
import { useNearbyBreweriesLazyQuery as _useNearbyBreweriesLazyQuery } from "../graphql/autogenerate/hooks";
import { Utils } from "./breweries-query-helper-utils";
import { useRef } from "react";

type NearbyBreweriesLazyQueryReturnType = [
    (latitude: number, longitude: number) => void,
    {
        breweries: Brewery[];
        error?: Error;
        hasMore: boolean;
        loading: boolean;
        loadMore: () => void;
    }
];

export const useNearbyBreweriesLazyQuery = (): NearbyBreweriesLazyQueryReturnType => {
    const latitudeRef = useRef<number>();
    const longitudeRef = useRef<number>();

    const [
        getBreweries,
        { data, loading, error, fetchMore },
    ] = _useNearbyBreweriesLazyQuery();

    const breweries: Brewery[] =
        data?.breweries?.edges
            ?.filter(isBreweryNode)
            ?.map(({ node }) => node as Brewery) ?? [];

    const loadMore = () =>
        !!fetchMore
            ? fetchMore({
                  variables: {
                      cursor: data?.breweries?.pageInfo.endCursor,
                      latitude: latitudeRef.current,
                      longitude: longitudeRef.current,
                  },
                  updateQuery: Utils.fetchMoreBreweriesUpdateQuery,
              })
            : {};

    const hasMore = data?.breweries?.pageInfo?.hasNextPage ?? false;

    const _getBreweries = (latitude: number, longitude: number) => {
        latitudeRef.current = latitude;
        longitudeRef.current = longitude;
        getBreweries({
            variables: {
                latitude,
                longitude,
            },
        });
    };

    return [
        _getBreweries,
        {
            breweries,
            error,
            hasMore,
            loading,
            loadMore,
        },
    ];
};
