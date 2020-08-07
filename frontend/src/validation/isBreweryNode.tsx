import { Brewery } from "../queries/autogenerate/schemas";

export const isBreweryNode = (b?: any): b is { node: Brewery } => !!b?.node;
