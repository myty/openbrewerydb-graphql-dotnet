import * as Types from "./operations";

import * as Apollo from "@apollo/client";
const gql = Apollo.gql;

export const BreweryFieldsFragmentDoc = gql`
    fragment BreweryFields on Brewery {
        name
        brewery_id
        street
        city
        state
        country
        website_url
        brewery_type
        tag_list
        phone
        latitude
        longitude
    }
`;
export const BreweriesDocument = gql`
    query Breweries($cursor: String) {
        breweries(first: 25, after: $cursor)
            @connection(key: "default_breweries") {
            totalCount
            pageInfo {
                startCursor
                hasNextPage
                hasPreviousPage
                endCursor
            }
            edges {
                cursor
                node {
                    id
                    ...BreweryFields
                }
            }
        }
    }
    ${BreweryFieldsFragmentDoc}
`;

/**
 * __useBreweriesQuery__
 *
 * To run a query within a React component, call `useBreweriesQuery` and pass it any options that fit your needs.
 * When your component renders, `useBreweriesQuery` returns an object from Apollo Client that contains loading, error, and data properties
 * you can use to render your UI.
 *
 * @param baseOptions options that will be passed into the query, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options;
 *
 * @example
 * const { data, loading, error } = useBreweriesQuery({
 *   variables: {
 *      cursor: // value for 'cursor'
 *   },
 * });
 */
export function useBreweriesQuery(
    baseOptions?: Apollo.QueryHookOptions<
        Types.BreweriesQuery,
        Types.BreweriesQueryVariables
    >
) {
    return Apollo.useQuery<Types.BreweriesQuery, Types.BreweriesQueryVariables>(
        BreweriesDocument,
        baseOptions
    );
}
export function useBreweriesLazyQuery(
    baseOptions?: Apollo.LazyQueryHookOptions<
        Types.BreweriesQuery,
        Types.BreweriesQueryVariables
    >
) {
    return Apollo.useLazyQuery<
        Types.BreweriesQuery,
        Types.BreweriesQueryVariables
    >(BreweriesDocument, baseOptions);
}
export type BreweriesQueryHookResult = ReturnType<typeof useBreweriesQuery>;
export type BreweriesLazyQueryHookResult = ReturnType<
    typeof useBreweriesLazyQuery
>;
export type BreweriesQueryResult = Apollo.QueryResult<
    Types.BreweriesQuery,
    Types.BreweriesQueryVariables
>;
export const BreweryDocument = gql`
    query Brewery($id: ID!) {
        brewery: node(id: $id) {
            id
            ...BreweryFields
        }
    }
    ${BreweryFieldsFragmentDoc}
`;

/**
 * __useBreweryQuery__
 *
 * To run a query within a React component, call `useBreweryQuery` and pass it any options that fit your needs.
 * When your component renders, `useBreweryQuery` returns an object from Apollo Client that contains loading, error, and data properties
 * you can use to render your UI.
 *
 * @param baseOptions options that will be passed into the query, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options;
 *
 * @example
 * const { data, loading, error } = useBreweryQuery({
 *   variables: {
 *      id: // value for 'id'
 *   },
 * });
 */
export function useBreweryQuery(
    baseOptions?: Apollo.QueryHookOptions<
        Types.BreweryQuery,
        Types.BreweryQueryVariables
    >
) {
    return Apollo.useQuery<Types.BreweryQuery, Types.BreweryQueryVariables>(
        BreweryDocument,
        baseOptions
    );
}
export function useBreweryLazyQuery(
    baseOptions?: Apollo.LazyQueryHookOptions<
        Types.BreweryQuery,
        Types.BreweryQueryVariables
    >
) {
    return Apollo.useLazyQuery<Types.BreweryQuery, Types.BreweryQueryVariables>(
        BreweryDocument,
        baseOptions
    );
}
export type BreweryQueryHookResult = ReturnType<typeof useBreweryQuery>;
export type BreweryLazyQueryHookResult = ReturnType<typeof useBreweryLazyQuery>;
export type BreweryQueryResult = Apollo.QueryResult<
    Types.BreweryQuery,
    Types.BreweryQueryVariables
>;
export const BreweryByIdDocument = gql`
    query BreweryById($brewery_id: String!) {
        brewery: breweryById(brewery_id: $brewery_id) {
            id
            ...BreweryFields
        }
    }
    ${BreweryFieldsFragmentDoc}
`;

/**
 * __useBreweryByIdQuery__
 *
 * To run a query within a React component, call `useBreweryByIdQuery` and pass it any options that fit your needs.
 * When your component renders, `useBreweryByIdQuery` returns an object from Apollo Client that contains loading, error, and data properties
 * you can use to render your UI.
 *
 * @param baseOptions options that will be passed into the query, supported options are listed on: https://www.apollographql.com/docs/react/api/react-hooks/#options;
 *
 * @example
 * const { data, loading, error } = useBreweryByIdQuery({
 *   variables: {
 *      brewery_id: // value for 'brewery_id'
 *   },
 * });
 */
export function useBreweryByIdQuery(
    baseOptions?: Apollo.QueryHookOptions<
        Types.BreweryByIdQuery,
        Types.BreweryByIdQueryVariables
    >
) {
    return Apollo.useQuery<
        Types.BreweryByIdQuery,
        Types.BreweryByIdQueryVariables
    >(BreweryByIdDocument, baseOptions);
}
export function useBreweryByIdLazyQuery(
    baseOptions?: Apollo.LazyQueryHookOptions<
        Types.BreweryByIdQuery,
        Types.BreweryByIdQueryVariables
    >
) {
    return Apollo.useLazyQuery<
        Types.BreweryByIdQuery,
        Types.BreweryByIdQueryVariables
    >(BreweryByIdDocument, baseOptions);
}
export type BreweryByIdQueryHookResult = ReturnType<typeof useBreweryByIdQuery>;
export type BreweryByIdLazyQueryHookResult = ReturnType<
    typeof useBreweryByIdLazyQuery
>;
export type BreweryByIdQueryResult = Apollo.QueryResult<
    Types.BreweryByIdQuery,
    Types.BreweryByIdQueryVariables
>;
