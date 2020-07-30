import React from "react";
import { NavLink } from "react-router-dom";

interface HeaderProps {
    title: string;
}

const MenuLink = ({ to, text }: { to: string; text: string }) => (
    <NavLink
        activeClassName="underline font-bold"
        className="block mt-4 lg:inline-block lg:mt-0 text-gray-900 hover:font-bold mr-4"
        to={to}>
        {text}
    </NavLink>
);

const Header = ({ title }: HeaderProps) => {
    return (
        <nav className="flex items-center justify-between flex-wrap bg-yellow-400 p-6 shadow-md">
            <div className="flex items-center flex-shrink-0 text-gray-900 mr-6">
                <NavLink
                    className="font-semibold text-xl tracking-tight hover:font-bold"
                    to="/">
                    {title}
                </NavLink>
            </div>
            <div className="block lg:hidden">
                <button className="flex items-center px-3 py-2 border rounded text-gray-800 border-gray-800 hover:text-white hover:border-white">
                    <svg
                        className="fill-current h-3 w-3"
                        viewBox="0 0 20 20"
                        xmlns="http://www.w3.org/2000/svg">
                        <title>Menu</title>
                        <path d="M0 3h20v2H0V3zm0 6h20v2H0V9zm0 6h20v2H0v-2z" />
                    </svg>
                </button>
            </div>
            <div className="w-full block flex-grow lg:flex lg:items-center lg:w-auto">
                <div className="text-sm lg:flex-grow">
                    <MenuLink to="nearby" text="Nearby" />
                    <MenuLink to="favorites" text="Favorites" />
                </div>
                <div>
                    <button className="inline-block text-sm px-4 py-2 leading-none border rounded text-gray-900 border-gray-900 hover:border-transparent hover:text-yellow-400 hover:bg-gray-900 mt-4 lg:mt-0">
                        Login
                    </button>
                </div>
            </div>
        </nav>
    );
};

export default Header;
