import React, { PropsWithChildren } from "react";

export const HeadingOne = ({ children }: PropsWithChildren<any>) => (
    <h1 className="text-2xl text-blue-700 leading-tight">{children}</h1>
);
