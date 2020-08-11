import React, { useState } from "react";
import GoogleMapReact, { Coords } from "google-map-react";
import { Brewery } from "../graphql/autogenerate/schemas";

interface BreweryMapProps {
    brewery: Brewery;
}

interface MarkerComponentInterface extends Partial<Coords> {
    text: string;
    primary: boolean;
}

const MarkerComponent = ({ text, primary }: MarkerComponentInterface) => {
    const className = `${
        primary ? "bg-blue-700" : "bg-gray-500"
    } w-64 p-2 -ml-8 text-base font-semibold text-center text-white bg-opacity-75 rounded left`;

    return (
        <div className={className}>
            <p className="w-full truncate">{text}</p>
        </div>
    );
};

export const BreweryMap = ({ brewery }: BreweryMapProps) => {
    const [warningDismissed, setWarningDismissed] = useState(false);

    if (!brewery.latitude || !brewery.longitude) {
        return <p className="mt-6">Location Not Found</p>;
    }

    if (
        process.env.REACT_APP_GOOGLE_MAPS_API_KEY ===
        process.env.REACT_APP_DEFAULT_API_KEY
    ) {
        if (warningDismissed) {
            return null;
        }

        return (
            <div
                className="relative px-4 py-3 text-red-700 bg-red-100 border border-red-400 rounded"
                role="alert">
                <strong className="font-bold">Google Maps</strong>
                <span className="block sm:inline">
                    Update your Google Maps API key in order to have the map be
                    visible.
                </span>
                <button
                    className="absolute top-0 bottom-0 right-0 inline px-4 py-3"
                    onClick={() => setWarningDismissed(true)}>
                    <svg
                        className="w-6 h-6 text-red-500 fill-current"
                        role="button"
                        xmlns="http://www.w3.org/2000/svg"
                        viewBox="0 0 20 20">
                        <title>Close</title>
                        <path d="M14.348 14.849a1.2 1.2 0 0 1-1.697 0L10 11.819l-2.651 3.029a1.2 1.2 0 1 1-1.697-1.697l2.758-3.15-2.759-3.152a1.2 1.2 0 1 1 1.697-1.697L10 8.183l2.651-3.031a1.2 1.2 0 1 1 1.697 1.697l-2.758 3.152 2.758 3.15a1.2 1.2 0 0 1 0 1.698z" />
                    </svg>
                </button>
            </div>
        );
    }

    const apiKey = process.env.REACT_APP_GOOGLE_MAPS_API_KEY ?? "";
    const { latitude: lat, longitude: lng, name: text } = brewery;

    const nearbyBreweries = !brewery.nearby
        ? []
        : brewery.nearby?.map((nb) => (
              <MarkerComponent
                  key={`marker_${nb?.id}`}
                  lat={nb?.latitude}
                  lng={nb?.longitude}
                  text={nb?.name!}
                  primary={false}
              />
          ));

    return (
        // Important! Always set the container height explicitly
        <div
            style={{
                height: "calc(100vh - 8rem)",
                width: "100%",
            }}>
            <GoogleMapReact
                bootstrapURLKeys={{ key: apiKey }}
                defaultCenter={{ lat, lng }}
                defaultZoom={15}>
                <MarkerComponent
                    lat={lat}
                    lng={lng}
                    text={text}
                    primary={true}
                />
                {nearbyBreweries}
            </GoogleMapReact>
        </div>
    );
};
