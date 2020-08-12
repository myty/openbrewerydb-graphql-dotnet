import React, { useState, useRef } from "react";
import { Brewery } from "../graphql/autogenerate/schemas";
import { useHasFocusObserver } from "../hooks/use-has-focus-observer";

interface AutocompleteTextboxProps<T = any> {
    className?: string;
    onEnter?: (text: string) => void;
    onTextChange: (text: string) => void;
    results: T[];
    renderResultOption: (option: T) => React.ReactNode;
}

export const AutocompleteTextbox: React.FC<AutocompleteTextboxProps<
    Brewery
>> = ({
    className,
    onEnter = () => {},
    onTextChange,
    results,
    renderResultOption,
}) => {
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

    const onKeyPressCapture = (evt: React.KeyboardEvent<HTMLElement>) => {
        if (evt.key === "Enter") {
            onEnter(searchText);
            changeText();
        }
    };

    return (
        <div
            ref={outsideDivRef}
            className={`h-0 overflow-visible lg:inline-block relative`}
            style={{ top: "-1.25rem", zIndex: 999 }}
            onKeyPressCapture={onKeyPressCapture}>
            <div
                className={`${className} rounded-lg border border-gray-200 shadow bg-white absolute`}>
                <input
                    type="text"
                    className={`w-full px-3 py-1 rounded-lg outline-none`}
                    placeholder="Search"
                    value={searchText}
                    onChange={onChange}
                />
                {results.length > 0 && (
                    <React.Fragment>
                        <div className="px-2 text-xs italic font-semibold bg-gray-100 border-t">
                            Suggestions
                        </div>
                        <div className="w-full bg-white rounded-lg">
                            {results.map((b) => renderResultOption(b))}
                        </div>
                    </React.Fragment>
                )}
            </div>
        </div>
    );
};
