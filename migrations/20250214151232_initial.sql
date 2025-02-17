-- Add new schema named "atlas_schema_revisions"
CREATE SCHEMA "atlas_schema_revisions";
-- Create "cocktail" table
CREATE TABLE "public"."cocktail" ("id" serial NOT NULL, "name" text NOT NULL, "column_name" timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP, PRIMARY KEY ("id"));
-- Set comment to column: "column_name" on table: "cocktail"
COMMENT ON COLUMN "public"."cocktail"."column_name" IS 'created';
-- Create "sessions" table
CREATE TABLE "public"."sessions" ("token" text NOT NULL, "data" bytea NOT NULL, "expiry" timestamptz NOT NULL, PRIMARY KEY ("token"));
-- Create index "sessions_expiry_idx" to table: "sessions"
CREATE INDEX "sessions_expiry_idx" ON "public"."sessions" ("expiry");
-- Create "users" table
CREATE TABLE "public"."users" ("id" serial NOT NULL, "name" character varying(255) NOT NULL, "email" character varying(255) NOT NULL, "hashed_password" character(60) NOT NULL, "created" timestamptz NOT NULL, PRIMARY KEY ("id"), CONSTRAINT "users_uc_email" UNIQUE ("email"));
-- Create "atlas_schema_revisions" table
CREATE TABLE "atlas_schema_revisions"."atlas_schema_revisions" ("version" character varying NOT NULL, "description" character varying NOT NULL, "type" bigint NOT NULL DEFAULT 2, "applied" bigint NOT NULL DEFAULT 0, "total" bigint NOT NULL DEFAULT 0, "executed_at" timestamptz NOT NULL, "execution_time" bigint NOT NULL, "error" text NULL, "error_stmt" text NULL, "hash" character varying NOT NULL, "partial_hashes" jsonb NULL, "operator_version" character varying NOT NULL, PRIMARY KEY ("version"));
