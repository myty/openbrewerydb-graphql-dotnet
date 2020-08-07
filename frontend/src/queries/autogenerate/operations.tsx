import * as Types from "./schemas";

export type BreweriesQueryVariables = Types.Exact<{
    cursor?: Types.Maybe<Types.Scalars["String"]>;
}>;

export type BreweriesQuery = { __typename?: "BreweriesQuery" } & {
    breweries?: Types.Maybe<
        { __typename?: "BreweryConnection" } & Pick<
            Types.BreweryConnection,
            "totalCount"
        > & {
                pageInfo: { __typename?: "PageInfo" } & Pick<
                    Types.PageInfo,
                    | "startCursor"
                    | "hasNextPage"
                    | "hasPreviousPage"
                    | "endCursor"
                >;
                edges?: Types.Maybe<
                    Array<
                        { __typename?: "BreweryEdge" } & Pick<
                            Types.BreweryEdge,
                            "cursor"
                        > & {
                                node?: Types.Maybe<
                                    { __typename?: "Brewery" } & Pick<
                                        Types.Brewery,
                                        "id"
                                    > &
                                        BreweryFieldsFragment
                                >;
                            }
                    >
                >;
            }
    >;
};

export type BreweryQueryVariables = Types.Exact<{
    id: Types.Scalars["ID"];
}>;

export type BreweryQuery = { __typename?: "BreweriesQuery" } & {
    brewery?: Types.Maybe<
        { __typename?: "Brewery" } & Pick<Types.Brewery, "id"> &
            BreweryFieldsFragment
    >;
};

export type BreweryFieldsFragment = { __typename?: "Brewery" } & Pick<
    Types.Brewery,
    | "name"
    | "brewery_id"
    | "street"
    | "city"
    | "state"
    | "country"
    | "website_url"
    | "brewery_type"
    | "tag_list"
    | "phone"
    | "latitude"
    | "longitude"
>;
