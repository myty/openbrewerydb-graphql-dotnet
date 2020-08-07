import React from "react";
import { Brewery } from "../queries/autogenerate/schemas";

interface BreweryNavCardProps {
    brewery: Brewery;
}

export const BreweryNavCard = ({ brewery }: BreweryNavCardProps) => {
    return (
        <a
            href={`/breweries/${brewery.brewery_id}`}
            className="block w-full max-w-xl p-4 mx-auto my-4 overflow-hidden border border-gray-300 rounded-md shadow outline-none focus:shadow-outline focus:bg-gray-100">
            <div className="flex space-x-4">
                <div className="w-12 h-12 bg-gray-400 rounded-full"></div>
                <div className="flex-1 py-1 space-y-4">
                    <div className="w-3/4 px-3">
                        <div className="block w-full font-bold tracking-tighter truncate text-md">
                            {brewery.name}
                        </div>
                        <div className="block text-xs subpixel-antialiased text-opacity-50 uppercase">
                            {brewery.city}, {brewery.state}
                        </div>
                    </div>
                </div>
            </div>
        </a>
    );
};
