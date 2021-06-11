import React, { useState, useEffect } from "react";
import { Loading } from "../components/loading";
import InfiniteScroll from "react-infinite-scroller";
import { BreweryNavCard } from "../components/brewery-nav-card";
import { Brewery } from "../graphql/autogenerate/schemas";
import { useNearbyBreweries } from "../hooks/use-nearby-breweries";
import { Position } from "google-map-react";

export const NearbyPage = () => {
    const [position, setPosition] = useState<Position>();

    const { breweries, error, loading, hasMore, loadMore } = useNearbyBreweries({
        latitude: position?.lat,
        longitude: position?.lng,
    });

    useEffect(() => {
        navigator.geolocation.getCurrentPosition((p) => {
            setPosition({
                lat: p.coords.latitude,
                lng: p.coords.longitude,
            });
        });
    }, []);

    if (loading && breweries.length === 0) return <Loading />;
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
