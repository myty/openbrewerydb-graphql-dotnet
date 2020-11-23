import { Entity, PrimaryGeneratedColumn, Column } from "typeorm";
import "reflect-metadata";

export interface IReview {
    id: number;
    breweryId: number;
    userId: number;
    title: string;
    body: string;
}

@Entity({
    name: "brewery_reviews",
})
export class Review implements IReview {
    @PrimaryGeneratedColumn()
    id: number;

    @Column()
    breweryId: number;

    @Column()
    userId: number;

    @Column()
    title: string;

    @Column()
    body: string;
}
