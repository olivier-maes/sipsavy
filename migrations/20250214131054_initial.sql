-- Create "cocktail" table
CREATE TABLE "public"."cocktail" ("id" serial NOT NULL, "name" text NOT NULL, PRIMARY KEY ("id"));
-- Create "sessions" table
CREATE TABLE "public"."sessions" ("token" text NOT NULL, "data" bytea NOT NULL, "expiry" timestamptz NOT NULL, PRIMARY KEY ("token"));
-- Create index "sessions_expiry_idx" to table: "sessions"
CREATE INDEX "sessions_expiry_idx" ON "public"."sessions" ("expiry");
-- Create "users" table
CREATE TABLE "public"."users" ("id" serial NOT NULL, "name" character varying(255) NOT NULL, "email" character varying(255) NOT NULL, "hashed_password" character(60) NOT NULL, "created" timestamptz NOT NULL, PRIMARY KEY ("id"), CONSTRAINT "users_uc_email" UNIQUE ("email"));
