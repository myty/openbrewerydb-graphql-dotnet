import React, { useState, useRef } from "react";
import { Brewery } from "../graphql/autogenerate/schemas";
import { useHasFocusObserver } from "../hooks/use-has-focus-observer";

interface AutocompleteTextboxProps<T = any> {
    className?: string;
    onTextChange: (text: string) => void;
    results: T[];
    renderResultOption: (option: T) => React.ReactNode;
}

export const AutocompleteTextbox: React.FC<AutocompleteTextboxProps<
    Brewery
>> = ({ className, onTextChange, results, renderResultOption }) => {
    const [searchText, setSearchText] = useState("");
    const outsideDivRef = useRef<HTMLDivElement>(null);

    const onChange = (e: React.ChangeEvent<HTMLInputElement>) =>
        changeText(e.target.value);

    const changeText = (value?: string) => {
        setSearchText(value ?? "");
        onTextChange(value ?? "");
    };

    useHasFocusObserver((hasFocus) => {
        if (!hasFocus) {
            changeText();
        }
    }, outsideDivRef);

    return (
        <div
            ref={outsideDivRef}
            className={`h-0 overflow-visible lg:inline-block relative`}
            style={{ top: "-1.25rem", zIndex: 999 }}>
            <div
                className={`${className} rounded-lg border border-gray-200 shadow bg-white absolute`}>
                <input
                    type="text"
                    className={`w-full px-3 py-1 rounded-lg outline-none`}
                    placeholder="Search"
                    value={searchText}
                    onChange={onChange}
                />
                <div className="w-full bg-white rounded-lg">
                    {results.map((b) => renderResultOption(b))}
                </div>
            </div>
        </div>
    );
};
