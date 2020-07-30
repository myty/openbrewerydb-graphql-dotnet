import React from "react";
import { NavLink } from "react-router-dom";

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

const Header = ({ title }: HeaderProps) => {
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
                </div>
                <div>
                    <button className="inline-block px-4 py-2 mt-4 text-sm leading-none text-gray-900 border border-gray-900 rounded hover:border-transparent hover:text-yellow-400 hover:bg-gray-900 lg:mt-0">
                        Login
                    </button>
                </div>
            </div>
        </nav>
    );
};

export default Header;
