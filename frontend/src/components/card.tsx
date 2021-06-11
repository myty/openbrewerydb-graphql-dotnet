import { HTMLAttributes } from "react";
import React from "react";

export const Card: React.FC<HTMLAttributes<HTMLDivElement>> = ({
    className,
    children,
    ...args
}) => {
    const divClasses =
        "max-w-sm overflow-hidden border border-gray-300 rounded shadow-lg " + className;

    return (
        <div className={divClasses} {...args}>
            {children}
        </div>
    );
};
