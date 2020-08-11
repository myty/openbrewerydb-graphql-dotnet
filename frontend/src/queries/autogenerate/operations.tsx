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
                                        BreweryBaseFieldsFragment
                                >;
                            }
                    >
                >;
            }
    >;
};

export type NearbyBreweriesQueryVariables = Types.Exact<{
    cursor?: Types.Maybe<Types.Scalars["String"]>;
    latitude: Types.Scalars["Decimal"];
    longitude: Types.Scalars["Decimal"];
}>;

export type NearbyBreweriesQuery = { __typename?: "BreweriesQuery" } & {
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
                                        BreweryBaseFieldsFragment
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
            BreweryDetailFieldsFragment
    >;
};

export type BreweryByIdQueryVariables = Types.Exact<{
    brewery_id: Types.Scalars["String"];
}>;

export type BreweryByIdQuery = { __typename?: "BreweriesQuery" } & {
    brewery?: Types.Maybe<
        { __typename?: "Brewery" } & Pick<Types.Brewery, "id"> &
            BreweryDetailFieldsFragment
    >;
};

export type BreweryBaseFieldsFragment = { __typename?: "Brewery" } & Pick<
    Types.Brewery,
    | "name"
    | "brewery_id"
    | "street"
    | "city"
    | "state"
    | "country"
    | "website_url"
    | "brewery_type"
>;

export type BreweryDetailFieldsFragment = { __typename?: "Brewery" } & Pick<
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
> & {
        nearby?: Types.Maybe<
            Array<
                Types.Maybe<
                    { __typename?: "Brewery" } & Pick<
                        Types.Brewery,
                        "id" | "latitude" | "longitude"
                    > &
                        BreweryBaseFieldsFragment
                >
            >
        >;
    };
