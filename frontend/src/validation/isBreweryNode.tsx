import { Brewery, BreweryEdge } from "../graphql/autogenerate/schemas";

export const isBreweryNode = (
    b: BreweryEdge,
): b is Required<BreweryEdge> & { node: Brewery } => {
    return b?.node != null;
};
