import React from "react";
import { Loading } from "../components/loading";
import { BreweryMap } from "../components/map";
import { HeadingOne } from "../components/heading-1";
import { useParams } from "react-router-dom";
import { useBreweryQuery } from "../queries/autogenerate/hooks";

export const BreweryPage = () => {
    const { id } = useParams();

    const { loading, error, data } = useBreweryQuery({
        variables: { id },
    });

    if (loading) return <Loading />;
    if (error) return <p>Error :(</p>;

    if (!data?.brewery) {
        return (
            <h1 className="text-2xl font-semibold leading-tight text-yellow-900">
                Brewery Not Found
            </h1>
        );
    }

    const brewery = data?.brewery;

    return (
        <React.Fragment>
            <HeadingOne>{brewery.name}</HeadingOne>
            <p className="text-base font-semibold leading-normal text-yellow-700">{`${brewery.city}, ${brewery.state}`}</p>
            <BreweryMap
                lng={brewery.longitude}
                lat={brewery.latitude}
                text={brewery.name}
            />
        </React.Fragment>
    );
};
