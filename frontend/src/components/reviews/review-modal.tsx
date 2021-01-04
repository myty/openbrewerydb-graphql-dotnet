import React, { useEffect, useState } from "react";
import { Card } from "../card";
import { Modal, ModalProps } from "../modal";

type ReviewModalProps = Omit<ModalProps, "title"> & { breweryId: string };

export const ReviewModal = (props: ReviewModalProps) => {
    const newProps = {
        ...props,
        onAction: async () => {
            if (props.onAction) {
                props.onAction();
            }

            // await handleSubmitAction();
        },
    };

    const { showModal } = props;

    // const [createReview] = useCreateReviewMutation();

    const [title, setTitle] = useState<string>("");
    const [review, setReview] = useState<string>("");

    // const handleSubmitAction = useCallback(async () => {
    //     await createReview({
    //         variables: {
    //             createReviewInput: {
    //                 clientMutationId: v4(),
    //                 subject: title,
    //                 body: review,
    //                 breweryId: props.breweryId,
    //             },
    //         },
    //     });
    // }, [createReview, props.breweryId, title, review]);

    useEffect(() => {
        if (!showModal) {
            setReview("");
            setTitle("");
        }
    }, [showModal]);

    return (
        <Modal title="Add a Review!!!" {...newProps}>
            <div className="mb-4">
                <label className="block text-sm font-bold">Title</label>
                <input
                    className="block w-full p-1 border border-gray-400 rounded-md shadow-inner"
                    type="text"
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                />

                <label className="block mt-2 text-sm font-bold">Review</label>
                <textarea
                    value={review}
                    className="block w-full h-12 p-1 border border-gray-400 rounded-md shadow-inner"
                    onChange={(e) => setReview(e.target.value)}
                />
            </div>
        </Modal>
    );
};

export const BreweryReviewList = ({
    reviews,
}: {
    // reviews?: Maybe<Review>[] | null;
    reviews?: any[] | null;
}) => {
    if (reviews == null) {
        return null;
    }

    return (
        <>
            {reviews.map((r) => (
                <div>
                    {r?.subject}
                    <br />
                    {r?.body}
                </div>
            ))}
        </>
    );
};

export const BreweryReviews = ({
    id,
    external_id,
    reviews,
}: {
    id: string;
    external_id: string;
    reviews?: any[] | null;
    // reviews?: Maybe<Review>[] | null;
}) => {
    const [showModal, setShowModal] = useState(false);

    return (
        <React.Fragment>
            <Card className="mt-4">
                <div className="px-6 py-4">
                    <div className="mb-2 text-xl font-bold">Reviews</div>
                    <button
                        className="text-base text-gray-700 hover:text-orange-600"
                        onClick={() => setShowModal(true)}>
                        Be the first to leave a review!
                    </button>
                </div>
                <BreweryReviewList reviews={reviews} />
            </Card>
            <ReviewModal
                breweryId={id}
                showModal={showModal}
                onAction={() => setShowModal(false)}
                onClose={() => setShowModal(false)}
            />
        </React.Fragment>
    );
};
