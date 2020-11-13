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
        id: { primaryKey: true, autoIncrement: true },
        body: DataTypes.STRING,
        breweryId: DataTypes.BIG_INTEGER,
        title: DataTypes.STRING,
        userId: DataTypes.BIG_INTEGER,
    };

    id!: number;
    breweryId!: number;
    userId!: number;
    title!: string;
    body!: string;
}
