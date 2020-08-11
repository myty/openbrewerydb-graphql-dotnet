import React, { useState, useEffect } from "react";
import { Loading } from "../components/loading";
import { HeadingOne } from "../components/heading-1";
import InfiniteScroll from "react-infinite-scroller";
import { BreweryNavCard } from "../components/brewery-nav-card";
import { Services } from "../services/breweries-query";
import { Brewery } from "../queries/autogenerate/schemas";

export const NearbyPage = () => {
    const [position, setPosition] = useState<Position>();

    useEffect(() => {
        navigator.geolocation.getCurrentPosition(setPosition);
    }, []);

    const {
        breweries,
        error,
        loading,
        hasMore,
        loadMore,
    } = Services.useNearbyBreweriesQuery(
        position?.coords?.latitude ?? 0,
        position?.coords?.longitude ?? 0
    );

    if (!position || loading) return <Loading />;
    if (error) return <p>Error :(</p>;

    if (breweries.length === 0) {
        return <HeadingOne>There are no breweries to display.</HeadingOne>;
    }

    return (
        <InfiniteScroll
            loadMore={loadMore}
            hasMore={hasMore}
            loader={<Loading key={0} />}>
            {breweries.map((b: Brewery) => (
                <BreweryNavCard
                    key={b.id}
                    brewery={b}
                    showDistanceFromPosition={true}
                    currentPosition={position}
                />
            ))}
        </InfiniteScroll>
    );
};
