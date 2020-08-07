import React from "react";
import GoogleMapReact, { Coords } from "google-map-react";

interface MarkerComponentInterface extends Partial<Coords> {
    text: string;
}

const MarkerComponent = ({ text }: MarkerComponentInterface) => (
    <div className="w-64 p-2 -ml-8 text-base font-semibold text-center text-white bg-blue-700 bg-opacity-75 rounded lef">
        <p className="w-full truncate">{text}</p>
    </div>
);

export const BreweryMap = ({ lat, lng, text }: MarkerComponentInterface) => {
    if (!lat || !lng) {
        return <p className="mt-6">Location Not Found</p>;
    }

    const apiKey = process.env.REACT_APP_GOOGLE_MAPS_API_KEY ?? "";

    return (
        // Important! Always set the container height explicitly
        <div
            style={{
                marginTop: "15px",
                height: "calc(100vh - 15rem)",
                width: "100%",
            }}>
            <GoogleMapReact
                bootstrapURLKeys={{ key: apiKey }}
                defaultCenter={{ lat, lng }}
                defaultZoom={15}>
                <MarkerComponent lat={lat} lng={lng} text={text} />
            </GoogleMapReact>
        </div>
    );
};
