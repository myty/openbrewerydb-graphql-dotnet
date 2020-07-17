import React from "react";

interface LoadingProps {
    loadingText?: string;
}

export const Loading = ({ loadingText }: LoadingProps) => {
    return (
        <div className="container mx-auto">
            <p className="text-base text-gray-700 leading-normal">{loadingText ?? "Loading..."}</p>
        </div>
    );
};
