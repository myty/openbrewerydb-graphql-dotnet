import React from "react";

interface LoadingProps {
    loadingText?: string;
}

export const Loading = ({ loadingText }: LoadingProps) => {
    return (
        <div className="block w-full max-w-xl p-4 mx-auto font-semibold text-center">
            <svg
                className="inline-block w-4 h-4 mr-2 text-gray-900 fill-current animate-spin"
                xmlns="http://www.w3.org/2000/svg"
                viewBox="0 0 20 20">
                <path d="M17.584,9.372h2c-0.065-1.049-0.293-2.053-0.668-2.984L17.16,7.402C17.384,8.025,17.531,8.685,17.584,9.372zM14.101,1.295c-0.955-0.451-1.99-0.757-3.086-0.87v2.021c0.733,0.097,1.433,0.295,2.084,0.585L14.101,1.295z M16.242,5.622l1.741-1.005c-0.591-0.878-1.33-1.645-2.172-2.285l-1.006,1.742C15.354,4.52,15.836,5.042,16.242,5.622z M10.014,17.571c-4.197,0-7.6-3.402-7.6-7.6c0-3.858,2.877-7.036,6.601-7.526V0.424c-4.833,0.5-8.601,4.583-8.601,9.547c0,5.303,4.298,9.601,9.601,9.601c4.824,0,8.807-3.563,9.486-8.2H17.48C16.822,14.899,13.732,17.571,10.014,17.571z" />
            </svg>
            {loadingText ?? "Loading..."}
        </div>
    );
};
