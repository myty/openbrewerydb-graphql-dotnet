module.exports = {
    purge: ["./src/**/*.{js,jsx,ts,tsx}", "./public/index.html"],
    darkMode: false, // or 'media' or 'class'
    theme: {
        extend: {
            boxShadow: {
                "outline-yellow": "0 0 0 3px rgb(236, 201, 75, 0.5)",
            },
        },
    },
    variants: {
        outline: ["focus", "responsive", "hover"],
    },
    plugins: [],
};
