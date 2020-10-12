export type IdType = { id: unknown };

export type InputType<T extends IdType> = {
    input: Omit<T, "id"> & {
        clientMutationId: string;
    };
};

export type PayloadType<T> = T & {
    clientMutationId: string;
};
