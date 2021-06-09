import React from "react";
import { Loading } from "../components/loading";
import { HeadingOne } from "../components/heading-1";
import InfiniteScroll from "react-infinite-scroller";
import { BreweryNavCard } from "../components/brewery-nav-card";
import { Brewery } from "../graphql/autogenerate/schemas";
import { useSearchQuery } from "../services/search-breweries-query";
import { useSearchParams } from "react-router-dom";

export const SearchPage = () => {
    const [params] = useSearchParams();

    const searchTerm = params.get("q") ?? "";

    const { breweries, error, loading, hasMore, loadMore, totalResults } =
        useSearchQuery(searchTerm);

    if (loading) return <Loading />;
    if (error) return <p>Error :(</p>;

    if (breweries.length === 0) {
        return <HeadingOne>There are no breweries to display.</HeadingOne>;
    }

    return (
        <React.Fragment>
            <div className="block w-full max-w-xl mx-auto my-4 font-bold">
                {totalResults} results returned for the seach term: {searchTerm}
            </div>
            <InfiniteScroll
                loadMore={loadMore}
                hasMore={hasMore}
                loader={<Loading key={0} />}>
                {breweries.map((b: Brewery) => (
                    <BreweryNavCard key={b.id} brewery={b} />
                ))}
            </InfiniteScroll>
        </React.Fragment>
    );
};
