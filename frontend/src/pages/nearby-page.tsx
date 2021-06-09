import React, { useState, useEffect } from "react";
import { Loading } from "../components/loading";
import InfiniteScroll from "react-infinite-scroller";
import { BreweryNavCard } from "../components/brewery-nav-card";
import { Brewery } from "../graphql/autogenerate/schemas";
import { useNearbyBreweriesLazyQuery } from "../services/nearby-breweries-query";
import { Position } from "google-map-react";

export const NearbyPage = () => {
    const [position, setPosition] = useState<Position>();

    const [getNearbyBreweries, { breweries, error, loading, hasMore, loadMore }] =
        useNearbyBreweriesLazyQuery();

    useEffect(() => {
        navigator.geolocation.getCurrentPosition((p) => {
            getNearbyBreweries(p.coords.latitude, p.coords.longitude);
            setPosition({
                lat: p.coords.latitude,
                lng: p.coords.longitude,
            });
        });
    }, [getNearbyBreweries]);

    if (loading || breweries.length === 0) return <Loading />;
    if (error) return <p>Error :(</p>;

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
