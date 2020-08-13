module.exports = {
    purge: [
        "src/**/*.js",
        "src/**/*.jsx",
        "src/**/*.ts",
        "src/**/*.tsx",
        "public/**/*.html",
    ],
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
