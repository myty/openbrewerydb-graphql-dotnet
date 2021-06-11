import React from "react";
import { Loading } from "../components/loading";
import { HeadingOne } from "../components/heading-1";
import InfiniteScroll from "react-infinite-scroller";
import { BreweryNavCard } from "../components/brewery-nav-card";
import { Brewery } from "../graphql/autogenerate/schemas";
import { useBreweries } from "../hooks/use-breweries";

export const HomePage = () => {
    const { breweries, error, loading, hasMore, loadMore } = useBreweries();

    if (loading) return <Loading />;
    if (error) return <p>Error :(</p>;

    if (breweries.length === 0) {
        return <HeadingOne>There are no breweries to display.</HeadingOne>;
    }

    return (
        <InfiniteScroll
            loadMore={loadMore}
            hasMore={hasMore}
            loader={<Loading key={0} />}
            threshold={500}>
            {breweries.map((b: Brewery) => (
                <BreweryNavCard key={b.id} brewery={b} />
            ))}
        </InfiniteScroll>
    );
};
