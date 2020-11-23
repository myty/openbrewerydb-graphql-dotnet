import {MigrationInterface, QueryRunner} from "typeorm";

export class InitialDatabase1606162241162 implements MigrationInterface {
    name = 'InitialDatabase1606162241162'

    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`CREATE TABLE "brewery_reviews" ("id" SERIAL NOT NULL, "breweryId" integer NOT NULL, "userId" integer NOT NULL, "title" character varying NOT NULL, "body" character varying NOT NULL, CONSTRAINT "PK_10e3e8fc79468a5133553c09999" PRIMARY KEY ("id"))`);
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`DROP TABLE "brewery_reviews"`);
    }

}
