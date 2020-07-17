import React from "react";
import GoogleMapReact, { Coords } from "google-map-react";

interface MarkerComponentInterface extends Partial<Coords> {
    text: string;
}

const MarkerComponent = ({ text }: MarkerComponentInterface) => (
    <div>
        <p className="font-medium">{text}</p>
    </div>
);

export const BreweryMap = ({ lat, lng, text }: MarkerComponentInterface) => {
    if (!lat || !lng) {
        return <p className="mt-6">Location Not Found</p>;
    }

    return (
        // Important! Always set the container height explicitly
        <div
            style={{
                marginTop: "15px",
                height: "calc(100vh - 15rem)",
                width: "100%",
            }}>
            <GoogleMapReact
                bootstrapURLKeys={{
                    key: "AIzaSyBIxW-4EqrBnsJ3l-_qaT1RyhVsKcjoe1k",
                }}
                defaultCenter={{ lat, lng }}
                defaultZoom={15}>
                <MarkerComponent lat={lat} lng={lng} text={text} />
            </GoogleMapReact>
        </div>
    );
};
