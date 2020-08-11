export type Maybe<T> = T | null;
export type Exact<T extends { [key: string]: unknown }> = {
    [K in keyof T]: T[K];
};
/** All built-in and custom scalars, mapped to their actual values */
export type Scalars = {
    ID: string;
    String: string;
    Boolean: boolean;
    Int: number;
    Float: number;
    /** The multiplier path scalar represents a valid GraphQL multiplier path string. */
    MultiplierPath: any;
    PaginationAmount: any;
    /** The built-in `Decimal` scalar type. */
    Decimal: any;
    /** The `DateTime` scalar represents an ISO-8601 compliant date time type. */
    DateTime: any;
};

export type Mutation = {
    __typename?: "Mutation";
    createBrewery?: Maybe<CreateBreweryPayload>;
    updateBrewery?: Maybe<UpdateBreweryPayload>;
};

export type MutationCreateBreweryArgs = {
    input?: Maybe<CreateBreweryInput>;
};

export type MutationUpdateBreweryArgs = {
    input?: Maybe<UpdateBreweryInput>;
};

/** The node interface is implemented by entities that have a global unique identifier. */
export type Node = {
    id: Scalars["ID"];
};

export type BreweriesQuery = {
    __typename?: "BreweriesQuery";
    breweries?: Maybe<BreweryConnection>;
    breweryById?: Maybe<Brewery>;
    nearbyBreweries?: Maybe<BreweryConnection>;
    node?: Maybe<Node>;
};

export type BreweriesQueryBreweriesArgs = {
    after?: Maybe<Scalars["String"]>;
    before?: Maybe<Scalars["String"]>;
    brewery_id?: Maybe<Scalars["String"]>;
    city?: Maybe<Scalars["String"]>;
    first?: Maybe<Scalars["PaginationAmount"]>;
    last?: Maybe<Scalars["PaginationAmount"]>;
    name?: Maybe<Scalars["String"]>;
    search?: Maybe<Scalars["String"]>;
    sort?: Maybe<Array<Maybe<Scalars["String"]>>>;
    state?: Maybe<Scalars["String"]>;
    tags?: Maybe<Array<Maybe<Scalars["String"]>>>;
    type?: Maybe<Scalars["String"]>;
};

export type BreweriesQueryBreweryByIdArgs = {
    brewery_id: Scalars["String"];
};

export type BreweriesQueryNearbyBreweriesArgs = {
    after?: Maybe<Scalars["String"]>;
    before?: Maybe<Scalars["String"]>;
    first?: Maybe<Scalars["PaginationAmount"]>;
    last?: Maybe<Scalars["PaginationAmount"]>;
    latitude: Scalars["Decimal"];
    longitude: Scalars["Decimal"];
};

export type BreweriesQueryNodeArgs = {
    id: Scalars["ID"];
};

/** A brewery of beer */
export type Brewery = Node & {
    __typename?: "Brewery";
    /** Friendly id for Brewery */
    brewery_id: Scalars["String"];
    /** Type of Brewery */
    brewery_type: Scalars["String"];
    /** The city of the brewery */
    city?: Maybe<Scalars["String"]>;
    /** The country of origin for the brewery */
    country?: Maybe<Scalars["String"]>;
    createdAt: Scalars["DateTime"];
    id: Scalars["ID"];
    /** Latitude portion of lat/long coordinates */
    latitude?: Maybe<Scalars["Decimal"]>;
    /** Longitude portion of lat/long coordinates */
    longitude?: Maybe<Scalars["Decimal"]>;
    /** Name of brewery */
    name: Scalars["String"];
    nearby?: Maybe<Array<Maybe<Brewery>>>;
    /** The phone number for the brewery */
    phone?: Maybe<Scalars["String"]>;
    /** The state of the brewery */
    postal_code?: Maybe<Scalars["String"]>;
    /** The state of the brewery */
    state?: Maybe<Scalars["String"]>;
    /** The street of the brewery */
    street?: Maybe<Scalars["String"]>;
    /** Tags that have been attached to the brewery */
    tag_list: Array<Maybe<Scalars["String"]>>;
    /** Date timestamp of the last time the record was updated */
    updated_at: Scalars["DateTime"];
    /** Website address for the brewery */
    website_url?: Maybe<Scalars["String"]>;
};

/** A brewery of beer */
export type BreweryNearbyArgs = {
    within?: Maybe<Scalars["Int"]>;
};

/** A connection to a list of items. */
export type BreweryConnection = {
    __typename?: "BreweryConnection";
    /** A list of edges. */
    edges?: Maybe<Array<BreweryEdge>>;
    /** A flattened list of the nodes. */
    nodes?: Maybe<Array<Maybe<Brewery>>>;
    /** Information to aid in pagination. */
    pageInfo: PageInfo;
    totalCount: Scalars["Int"];
};

/** Information about pagination in a connection. */
export type PageInfo = {
    __typename?: "PageInfo";
    /** When paginating forwards, the cursor to continue. */
    endCursor?: Maybe<Scalars["String"]>;
    /** Indicates whether more edges exist following the set defined by the clients arguments. */
    hasNextPage: Scalars["Boolean"];
    /** Indicates whether more edges exist prior the set defined by the clients arguments. */
    hasPreviousPage: Scalars["Boolean"];
    /** When paginating backwards, the cursor to continue. */
    startCursor?: Maybe<Scalars["String"]>;
};

/** An edge in a connection. */
export type BreweryEdge = {
    __typename?: "BreweryEdge";
    /** A cursor for use in pagination. */
    cursor: Scalars["String"];
    /** The item at the end of the edge. */
    node?: Maybe<Brewery>;
};

export type CreateBreweryPayload = {
    __typename?: "CreateBreweryPayload";
    brewery?: Maybe<Brewery>;
    /** Relay Client Mutation Id */
    clientMutationId: Scalars["String"];
    errors?: Maybe<Array<Maybe<UserError>>>;
};

export type CreateBreweryInput = {
    brewery_id: Scalars["String"];
    /** Type of Brewery */
    brewery_type: Scalars["String"];
    /** The city of the brewery */
    city?: Maybe<Scalars["String"]>;
    /** Relay Client Mutation Id */
    clientMutationId: Scalars["String"];
    /** The country of origin for the brewery */
    country?: Maybe<Scalars["String"]>;
    /** Latitude portion of lat/long coordinates */
    latitude?: Maybe<Scalars["Decimal"]>;
    /** Longitude portion of lat/long coordinates */
    longitude?: Maybe<Scalars["Decimal"]>;
    /** Name of brewery */
    name: Scalars["String"];
    /** The phone number for the brewery */
    phone?: Maybe<Scalars["String"]>;
    /** The postal code of the brewery */
    postal_code?: Maybe<Scalars["String"]>;
    /** The state of the brewery */
    state: Scalars["String"];
    /** The street of the brewery */
    street?: Maybe<Scalars["String"]>;
    /** Tags that have been attached to the brewery */
    tag_list?: Maybe<Array<Maybe<Scalars["String"]>>>;
    /** Website address for the brewery */
    website_url?: Maybe<Scalars["String"]>;
};

export type UpdateBreweryInput = {
    brewery_id: Scalars["String"];
    /** Type of Brewery */
    brewery_type: Scalars["String"];
    /** The city of the brewery */
    city?: Maybe<Scalars["String"]>;
    /** Relay Client Mutation Id */
    clientMutationId: Scalars["String"];
    /** The country of origin for the brewery */
    country?: Maybe<Scalars["String"]>;
    id: Scalars["ID"];
    /** Latitude portion of lat/long coordinates */
    latitude?: Maybe<Scalars["Decimal"]>;
    /** Longitude portion of lat/long coordinates */
    longitude?: Maybe<Scalars["Decimal"]>;
    /** Name of brewery */
    name: Scalars["String"];
    /** The phone number for the brewery */
    phone?: Maybe<Scalars["String"]>;
    /** The postal code of the brewery */
    postal_code?: Maybe<Scalars["String"]>;
    /** The state of the brewery */
    state: Scalars["String"];
    /** The street of the brewery */
    street?: Maybe<Scalars["String"]>;
    /** Tags that have been attached to the brewery */
    tag_list?: Maybe<Array<Maybe<Scalars["String"]>>>;
    /** Website address for the brewery */
    website_url?: Maybe<Scalars["String"]>;
};

export type UpdateBreweryPayload = {
    __typename?: "UpdateBreweryPayload";
    brewery?: Maybe<Brewery>;
    /** Relay Client Mutation Id */
    clientMutationId: Scalars["String"];
    errors?: Maybe<Array<Maybe<UserError>>>;
};

export type UserError = {
    __typename?: "UserError";
    code?: Maybe<Scalars["String"]>;
    message?: Maybe<Scalars["String"]>;
};
