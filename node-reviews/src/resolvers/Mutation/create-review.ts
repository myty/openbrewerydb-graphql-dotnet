import { getConnection } from "typeorm";
import { IReview, Review } from "../../entity/Review";
import { PayloadType } from "../../types";
import { CreateReviewInputType } from "../../types";

export const createReview = async (
    _: unknown,
    {
        input: { clientMutationId, breweryId, title, body, userId },
    }: CreateReviewInputType<IReview>
): Promise<PayloadType<{ review: IReview }>> => {
    const connection = getConnection();

    const reviewRepository = connection.getRepository(Review);

    const review: Omit<IReview, "id"> = {
        body,
        breweryId,
        title,
        userId,
    };

    const result = await reviewRepository.insert(review);
    const identifiers = result.identifiers as Array<Pick<IReview, "id">>;

    return {
        review: {
            ...review,
            ...identifiers[0],
        },
        clientMutationId,
    };
};
