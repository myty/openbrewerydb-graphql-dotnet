import { getConnection } from "typeorm";
import { IReview, Review } from "../../entity/Review";
import { DeleteReviewInputType, PayloadType } from "../../types";

export const deleteReview = async (
    _: unknown,
    {
        input: { clientMutationId, id },
    }: DeleteReviewInputType<IReview>
): Promise<PayloadType<{ deleted: boolean }>> => {
    const connection = getConnection();

    const reviewRepository = connection.getRepository(Review);

    const result = await reviewRepository.delete(id);
    const deleted = result.affected > 0;

    return {
        deleted,
        clientMutationId,
    };
};
