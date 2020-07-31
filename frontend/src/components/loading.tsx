import React from "react";

interface LoadingProps {
    loadingText?: string;
}

export const Loading = ({ loadingText }: LoadingProps) => {
    return (
        <div className="block w-full max-w-xl p-4 mx-auto font-semibold text-center">
            {loadingText ?? "Loading..."}
        </div>
    );
};
