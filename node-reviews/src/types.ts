export type IdType = { id: unknown };

export type CreateReviewInputType<T extends IdType> = {
    input: Omit<T, "id"> & {
        clientMutationId: string;
    };
};

export type DeleteReviewInputType<T extends IdType> = {
    input: Pick<T, "id"> & {
        clientMutationId: string;
    };
};

export type PayloadType<T> = T & {
    clientMutationId: string;
};
