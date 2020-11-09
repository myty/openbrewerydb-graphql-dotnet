import { DataTypes, Model } from "../deps.ts";

export interface IReview {
    id: number;
    breweryId: number;
    userId: number;
    title: string;
    body: string;
}

export default class Review extends Model implements IReview {
    static table = "reviews";

    static fields = {
        id: { type: DataTypes.BIG_INTEGER, primaryKey: true },
        breweryId: DataTypes.BIG_INTEGER,
        userId: DataTypes.BIG_INTEGER,
        title: DataTypes.STRING,
        body: DataTypes.STRING,
    };

    // private static _reviews: Array<IReview> = [];
    // private static _nextId = Number(0);

    // static create(
    //     reviews: Array<Omit<IReview, "id">>
    // ): Promise<Array<IReview>> {
    //     const nextId = Review._nextId;
    //     Review._nextId += reviews.length;

    //     return new Promise((resolve, reject) => {
    //         const createdReviews = reviews.map((v, i) => {
    //             return {
    //                 ...v,
    //                 id: nextId + i,
    //             };
    //         });

    //         Review._reviews.push(...createdReviews);

    //         resolve(createdReviews);
    //     });
    // }

    // static find(id: number): Promise<IReview | null> {
    //     return Promise.resolve(
    //         Review._reviews.find((r) => r.id === id) ?? null
    //     );
    // }

    // static all(): Promise<Array<IReview>> {
    //     return Promise.resolve(Review._reviews);
    // }

    // static where(field: keyof IReview, value: unknown) {
    //     return {
    //         async get() {
    //             return await Promise.resolve(
    //                 Review._reviews.filter((r) => r[field] === value) ?? null
    //             );
    //         },
    //     };
    // }

    id!: number;
    breweryId!: number;
    userId!: number;
    title!: string;
    body!: string;
}
