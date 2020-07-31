import { useEffect, useState, useCallback } from "react";
import fetchGraphQL from "../services/fetch-graphql";

interface GraphQLQueryResult<TData = any> {
    data?: TData;
    error: boolean;
    fetchMore: (
        variables: any,
        processFetchMoreData: (previousResult: TData, result: TData) => TData
    ) => void;
    loading: boolean;
}

export function useGraphqlQuery<TData = any>(
    text: string,
    variables?: any
): GraphQLQueryResult<TData> {
    const [data, setData] = useState<TData>();
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(false);

    const fetchBrewery = useCallback(async () => {
        try {
            const results = await fetchGraphQL(text, variables);
            setData(results);
            setLoading(false);
        } catch (err) {
            setError(err);
            setLoading(false);
        }
    }, [text, variables]);

    useEffect(() => { fetchBrewery(); }, []);

    const fetchMore = useCallback(
        (
            fetchMoreVariables: any,
            processFetchMoreData: (
                previousResult: TData,
                result: TData
            ) => TData
        ) => {
            fetchGraphQL(text, fetchMoreVariables).then((results: TData) => {
                const newResults = processFetchMoreData(data!, results);
                setData(newResults);
            });
        },
        [data, text]
    );

    return {
        data,
        error,
        fetchMore,
        loading,
    };
}
