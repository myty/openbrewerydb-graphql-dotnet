import { Review, IReview } from "../../entity/Review";
import { getConnection } from "typeorm";

export const getReviews = async (
    parent: unknown,
    { breweryId }: IReview,
    context: unknown,
    info: unknown
): Promise<IReview[]> => {
    const connection = getConnection();

    const reviewRepository = connection.getRepository(Review);

    const reviews = await (breweryId == null
        ? reviewRepository.find()
        : reviewRepository.find({ breweryId }));

    // connection.close();

    return reviews;
};
