import React from "react";
import { Brewery } from "../graphql/autogenerate/schemas";
import { getDistance } from "geolib";
import { Position } from "google-map-react";

interface BreweryNavCardProps {
    brewery: Brewery;
    currentPosition?: Position;
    showDistanceFromPosition?: boolean;
}

const DistanceFromPosition = ({
    from,
    to,
}: {
    from?: Position;
    to: Brewery;
}) => {
    if (
        from?.lat == null ||
        from?.lng == null ||
        to?.latitude == null ||
        to?.longitude == null
    ) {
        return null;
    }

    // getDistance returns meters and this converts them to miles
    const distance = Math.round(
        getDistance(
            {
                latitude: from.lat,
                longitude: from.lng,
            },
            {
                latitude: to.latitude,
                longitude: to.longitude,
            }
        ) * 0.00062137
    );

    return (
        <div className="flex-1 w-4 pt-2 text-xl font-semibold text-right text-yellow-800">{`${distance} mi`}</div>
    );
};

export const BreweryNavCard = ({
    brewery,
    currentPosition,
    showDistanceFromPosition = false,
}: BreweryNavCardProps) => {
    return (
        <a
            href={`/breweries/${brewery.external_id}`}
            className="block w-full max-w-xl p-4 mx-auto my-4 overflow-hidden border border-gray-300 rounded-md shadow outline-none focus:shadow-outline focus:bg-gray-100">
            <div className="flex space-x-4">
                <div className="flex-grow py-1 space-y-4">
                    <div className="w-3/4 px-3">
                        <div className="block w-full font-bold tracking-tighter truncate text-md">
                            {brewery.name}
                        </div>
                        <div className="block text-xs subpixel-antialiased text-opacity-50 uppercase">
                            {brewery.city}, {brewery.state}
                        </div>
                    </div>
                </div>
                {showDistanceFromPosition && (
                    <DistanceFromPosition to={brewery} from={currentPosition} />
                )}
            </div>
        </a>
    );
};
