import { Review, IReview } from "../../entity/Review";
import { getConnection } from "typeorm";

export const getReview = async (
    parent: never,
    { id }: IReview,
    context: unknown,
    info: unknown): Promise<IReview> => {
    const connection = getConnection();
    const reviewRepository = connection.getRepository(Review);
    const review = await reviewRepository.findOne(id);

    if (review == null) {
        throw new Error(
            `Review with the id of '${id}' was not able to be found.`
        );
    }
    return review;
};
