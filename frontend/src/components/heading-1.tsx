import React, { PropsWithChildren } from "react";

export const HeadingOne = ({ children }: PropsWithChildren<any>) => (
    <h1 className="text-2xl leading-tight text-gray-800">{children}</h1>
);
