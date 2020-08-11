import React, {
    useState,
    useRef,
    useEffect,
    useCallback,
    RefObject,
} from "react";
import { NavLink } from "react-router-dom";
import { Brewery } from "../graphql/autogenerate/schemas";
import { useAutocompleteLazyQuery } from "../graphql/autogenerate/hooks";

interface HeaderProps {
    title: string;
}

const MenuLink = ({ to, text }: { to: string; text: string }) => (
    <NavLink
        activeClassName="underline font-bold"
        className="block mt-4 mr-4 text-gray-900 lg:inline-block lg:mt-0 hover:underline"
        to={to}>
        {text}
    </NavLink>
);

interface AutocompleteTextboxProps<T = any> {
    className?: string;
    onTextChange: (text: string) => void;
    results: T[];
    renderResultOption: (option: T) => React.ReactNode;
}

type UseHasFocusObserverFunction = (
    callback: (hasFocus: boolean) => void,
    elementRef: RefObject<HTMLElement>
) => void;

const useHasFocusObserver: UseHasFocusObserverFunction = (
    callback,
    elementRef
) => {
    const onChange = useCallback(
        (e: FocusEvent | MouseEvent) => {
            if (elementRef.current == null) {
                callback(false);
                return;
            }

            for (
                let el = e.target as (Node & ParentNode) | null;
                el;
                el = el.parentNode
            ) {
                if (el === elementRef.current) {
                    callback(true);
                    return;
                }
            }

            callback(false);
            return;
        },
        [callback, elementRef]
    );

    useEffect(() => {
        document.addEventListener("focusin", onChange);
        document.addEventListener("click", onChange);

        return () => {
            document.removeEventListener("focusin", onChange);
            document.removeEventListener("click", onChange);
        };
    }, [callback, onChange]);
};

const AutocompleteTextbox: React.FC<AutocompleteTextboxProps<Brewery>> = ({
    className,
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

const Header = ({ title }: HeaderProps) => {
    const [searchText, setSearchText] = useState("");
    const [
        getAutocompleteResults,
        { data, loading, error },
    ] = useAutocompleteLazyQuery();

    const onAutocompleteTextChange = (text: string) => {
        setSearchText(text);
        getAutocompleteResults({ variables: { search: text } });
    };

    const autoCompleteResults: Brewery[] =
        (searchText?.length ?? 0) >= 1 &&
        !loading &&
        !error &&
        (data?.breweries?.edges?.length ?? 0) >= 1
            ? data?.breweries?.edges?.map((b) => {
                  return b.node as Brewery;
              }) ?? []
            : [];

    const renderAutocompleteResult = (brewery: Brewery) => {
        return (
            <a
                className="block w-full px-3 py-1 text-left"
                href={`/breweries/${brewery.brewery_id}`}>
                <div className="font-bold">{brewery.name}</div>
                <div className="text-xs italic text-gray-700">
                    {brewery.city}, {brewery.state}
                </div>
            </a>
        );
    };

    return (
        <nav className="flex flex-wrap items-center justify-between p-6 bg-yellow-400 shadow-md">
            <div className="flex items-center flex-shrink-0 mr-6 text-gray-900">
                <NavLink
                    className="text-xl font-semibold tracking-tight hover:underline"
                    to="/">
                    {title}
                </NavLink>
            </div>
            <div className="block lg:hidden">
                <button className="flex items-center px-3 py-2 text-gray-800 border border-gray-800 rounded hover:text-white hover:border-white">
                    <svg
                        className="w-3 h-3 fill-current"
                        viewBox="0 0 20 20"
                        xmlns="http://www.w3.org/2000/svg">
                        <title>Menu</title>
                        <path d="M0 3h20v2H0V3zm0 6h20v2H0V9zm0 6h20v2H0v-2z" />
                    </svg>
                </button>
            </div>
            <div className="flex-grow block w-full lg:flex lg:items-center lg:w-auto">
                <div className="text-sm lg:flex-grow">
                    <MenuLink to="nearby" text="Nearby" />
                    <MenuLink to="favorites" text="Favorites" />
                    <AutocompleteTextbox
                        className="w-64"
                        onTextChange={onAutocompleteTextChange}
                        results={autoCompleteResults}
                        renderResultOption={renderAutocompleteResult}
                    />
                </div>
            </div>
        </nav>
    );
};

export default Header;
