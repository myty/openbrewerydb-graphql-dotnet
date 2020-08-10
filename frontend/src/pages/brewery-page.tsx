import React from "react";
import { Loading } from "../components/loading";
import { BreweryMap } from "../components/map";
import { useParams } from "react-router-dom";
import { useBreweryByIdQuery } from "../queries/autogenerate/hooks";
import { Brewery } from "../queries/autogenerate/schemas";
import { Card } from "../components/card";

const BreweryCard = ({ brewery }: { brewery: Brewery }) => {
    const breweryAddress = [brewery.street, brewery.city, brewery.state]
        .filter((b) => !!b)
        .join(", ");

    return (
        <Card>
            <div className="px-6 py-4">
                <div className="mb-2 text-xl font-bold">{brewery.name}</div>
                <p className="text-base text-gray-700">{`${breweryAddress}`}</p>
            </div>
            <div className="px-6 py-4">
                {brewery.tag_list.map((tag) => (
                    <span
                        key={`tag_${tag}`}
                        className="inline-block px-3 py-1 mr-2 text-sm font-semibold text-gray-700 bg-gray-200 rounded-full">
                        {tag}
                    </span>
                ))}
            </div>
        </Card>
    );
};

const BreweryReviews = ({
    brewery_id,
    reviews,
}: {
    brewery_id: string;
    reviews?: string[];
}) => {
    return (
        <Card className="mt-4">
            <div className="px-6 py-4">
                <div className="mb-2 text-xl font-bold">Reviews</div>
                <a
                    className="text-base text-gray-700 hover:text-orange-600"
                    href={`${brewery_id}/add-review`}>
                    Be the first to leave a review!
                </a>
            </div>
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
                        className="inline-block px-3 py-1 mb-2 mr-2 text-sm font-semibold text-gray-700 bg-gray-200 rounded-full hover:bg-yellow-600 hover:text-yellow-100"
                        href={b.brewery_id}>
                        {b.name}
                    </a>
                ))}
            </div>
        </Card>
    );
};

export const BreweryPage = () => {
    const { brewery_id } = useParams();

    const { loading, error, data } = useBreweryByIdQuery({
        variables: { brewery_id },
    });

    if (loading) return <Loading />;
    if (error) return <p>Error :(</p>;

    const brewery = data?.brewery;

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
                <BreweryCard brewery={brewery as Brewery} />
                <BreweryReviews brewery_id={brewery_id} />
                <NearbyBreweries breweries={brewery.nearby as Brewery[]} />
            </div>
            <div className="flex-grow">
                <BreweryMap
                    lng={brewery.longitude}
                    lat={brewery.latitude}
                    text={brewery.name}
                />
            </div>
        </div>
    );
};
