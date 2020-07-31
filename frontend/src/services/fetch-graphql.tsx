async function fetchGraphQL(text: string, variables: any) {
    // Fetch data from OpenBreweryDB GraphQL API:
    const response = await fetch("https://localhost:5001/graphql", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({
            query: text,
            variables,
        }),
    });

    // Get the response as JSON
    return await response.json();
}

export default fetchGraphQL;
