import React, { useState, useMemo } from "react";
import { Loading } from "../components/loading";
import { BreweryMap } from "../components/map";
import { useParams } from "react-router-dom";
import { useBreweryByExternalIdQuery } from "../graphql/autogenerate/hooks";
import { Brewery } from "../graphql/autogenerate/schemas";
import { Card } from "../components/card";
import { ExternalLinkIcon } from "@heroicons/react/solid";

const BreweryCard = ({ brewery }: { brewery: Brewery }) => {
    const [markedAsFavorite, setMarkedAsFavorite] = useState(false);

    const breweryAddress = [brewery.street, brewery.city, brewery.state]
        .filter((b) => !!b)
        .join(", ");

    const fillStarClass = useMemo(() => {
        if (markedAsFavorite) {
            return "text-yellow-500 hover:text-yellow-600";
        }

        return "text-white hover:text-gray-100";
    }, [markedAsFavorite]);

    return (
        <Card className="py-4">
            <div className="px-6">
                <div className="flex mb-2">
                    <div className="flex-grow text-xl font-bold">{brewery.name}</div>
                    <svg
                        role="button"
                        tabIndex={0}
                        onClick={() => setMarkedAsFavorite((v) => !v)}
                        viewBox="0 0 24 24"
                        className={`flex-none w-6 h-6 fill-current cursor-pointer outline-none ${fillStarClass}`}>
                        <path
                            stroke="#CBD5E0"
                            d="M8.128 19.825a1.586 1.586 0 0 1-1.643-.117 1.543 1.543 0 0 1-.53-.662 1.515 1.515 0 0 1-.096-.837l.736-4.247-3.13-3a1.514 1.514 0 0 1-.39-1.569c.09-.271.254-.513.475-.698.22-.185.49-.306.776-.35L8.66 7.73l1.925-3.862c.128-.26.328-.48.577-.633a1.584 1.584 0 0 1 1.662 0c.25.153.45.373.577.633l1.925 3.847 4.334.615c.29.042.562.162.785.348.224.186.39.43.48.704a1.514 1.514 0 0 1-.404 1.58l-3.13 3 .736 4.247c.047.282.014.572-.096.837-.111.265-.294.494-.53.662a1.582 1.582 0 0 1-1.643.117l-3.865-2-3.865 2z"
                        />
                    </svg>
                </div>
                {brewery.website_url && (
                    <a
                        className="block text-base text-yellow-700"
                        href={brewery.website_url}
                        rel="noopener noreferrer"
                        target="_blank">
                        {`${brewery.website_url}`}
                        <ExternalLinkIcon className="inline h-14 w-14" />
                    </a>
                )}
                <p className="text-base text-gray-700">{`${breweryAddress}`}</p>
            </div>
            {(brewery?.tag_list?.length ?? 0) >= 1 && (
                <div className="px-6 pt-4">
                    {brewery.tag_list.map((tag) => (
                        <span
                            key={`tag_${tag}`}
                            className="inline-block px-3 py-1 mr-2 text-sm font-semibold text-gray-700 bg-gray-200 rounded-full">
                            {tag}
                        </span>
                    ))}
                </div>
            )}
        </Card>
    );
};

const NearbyBreweries = ({ breweries }: { breweries?: Brewery[] }) => {
    if ((breweries?.length ?? 0) < 1) {
        return null;
    }

    return (
        <Card className="mt-4">
            <div className="px-6 py-4">
                <div className="text-xl font-bold">Nearby Breweries</div>
            </div>
            <div className="px-6 pb-4">
                {breweries?.map((b) => (
                    <a
                        key={`${b.id}`}
                        className="inline-block max-w-full px-3 py-1 mb-2 mr-2 text-sm font-semibold text-gray-700 truncate bg-gray-200 rounded-full outline-none hover:bg-yellow-600 hover:text-yellow-100 focus:shadow-outline"
                        href={b.external_id}>
                        {b.name}
                    </a>
                ))}
            </div>
        </Card>
    );
};

export const BreweryPage = () => {
    const { external_id } = useParams();

    const [result] = useBreweryByExternalIdQuery({
        variables: { external_id },
    });

    if (result.fetching) return <Loading />;
    if (result.error) return <p>Error :(</p>;

    const brewery: Brewery = result.data?.brewery as Brewery;

    if (!brewery) {
        return (
            <h1 className="text-2xl font-semibold leading-tight text-yellow-900">
                Brewery Not Found
            </h1>
        );
    }

    return (
        <div className="flex">
            <div className="flex-none pr-4 lg:w-1/4 xl:w-1/5">
                <BreweryCard brewery={brewery} />
                {/* <BreweryReviews
                    id={brewery.id}
                    brewery_id={brewery_id}
                    reviews={brewery.reviews}
                /> */}
                <NearbyBreweries breweries={brewery.nearby?.items as Brewery[]} />
            </div>
            <div className="flex-grow">
                <BreweryMap brewery={brewery} />
            </div>
        </div>
    );
};
