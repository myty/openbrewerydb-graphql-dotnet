import React, { useState, useEffect } from "react";
import { Loading } from "../components/loading";
import { useOnReviewAddedSubscription } from "../graphql/autogenerate/hooks";

interface ReviewInternal {
    id: string;
    subject?: string;
    body: string;
    breweryName: string;
}

export const ReviewPage = () => {
    const [reviews, setReviews] = useState<ReviewInternal[]>([]);
    const { data, error, loading } = useOnReviewAddedSubscription();

    useEffect(() => {
        if (data?.review == null) return;

        const newReview: ReviewInternal = {
            id: data?.review?.id?.toString(),
            subject: data?.review?.subject ?? "",
            body: data?.review?.body ?? "",
            breweryName: data?.review?.brewery?.name ?? "",
        };

        setReviews((r) => [newReview, ...r]);
    }, [data]);

    if (loading || reviews.length === 0) return <Loading />;
    if (error) return <p>Error :(</p>;

    return (
        <>
            {reviews.map((r: ReviewInternal) => (
                <div
                    key={r.id}
                    className="block w-full max-w-xl p-4 mx-auto my-4 overflow-hidden">
                    <div className="text-sm font-semibold">{r.breweryName}</div>
                    <div className="font-bold">{r.subject}</div>
                    <div>{r.body}</div>
                </div>
            ))}
        </>
    );
};
