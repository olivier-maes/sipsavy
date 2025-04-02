-- Add new schema named "atlas_schema_revisions"
CREATE SCHEMA "atlas_schema_revisions";
-- Add new schema named "public"
CREATE SCHEMA IF NOT EXISTS "public";
-- Set comment to schema: "public"
COMMENT ON SCHEMA "public" IS 'standard public schema';
-- Create "atlas_schema_revisions" table
CREATE TABLE "atlas_schema_revisions"."atlas_schema_revisions" ("version" character varying NOT NULL, "description" character varying NOT NULL, "type" bigint NOT NULL DEFAULT 2, "applied" bigint NOT NULL DEFAULT 0, "total" bigint NOT NULL DEFAULT 0, "executed_at" timestamptz NOT NULL, "execution_time" bigint NOT NULL, "error" text NULL, "error_stmt" text NULL, "hash" character varying NOT NULL, "partial_hashes" jsonb NULL, "operator_version" character varying NOT NULL, PRIMARY KEY ("version"));
-- Create "cocktails" table
CREATE TABLE "public"."cocktails" ("id" serial NOT NULL, "name" text NOT NULL, "created" timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP, PRIMARY KEY ("id"));
-- Create "sessions" table
CREATE TABLE "public"."sessions" ("token" text NOT NULL, "data" bytea NOT NULL, "expiry" timestamptz NOT NULL, PRIMARY KEY ("token"));
-- Create index "sessions_expiry_idx" to table: "sessions"
CREATE INDEX "sessions_expiry_idx" ON "public"."sessions" ("expiry");
-- Create "users" table
CREATE TABLE "public"."users" ("id" serial NOT NULL, "name" character varying(255) NOT NULL, "email" character varying(255) NOT NULL, "hashed_password" character(60) NOT NULL, "created" timestamptz NOT NULL, PRIMARY KEY ("id"), CONSTRAINT "users_uc_email" UNIQUE ("email"));
-- Create "ingredients" table
CREATE TABLE "public"."ingredients" ("id" serial NOT NULL, "name" text NOT NULL, PRIMARY KEY ("id"));
-- Create "cocktails_ingredients" table
CREATE TABLE "public"."cocktails_ingredients" ("cocktail_id" integer NOT NULL, "ingredient_id" integer NOT NULL, "amount" integer NOT NULL, "unit" text NOT NULL, PRIMARY KEY ("cocktail_id", "ingredient_id"), CONSTRAINT "cocktails_ingredients_cocktail_id_fkey" FOREIGN KEY ("cocktail_id") REFERENCES "public"."cocktails" ("id") ON UPDATE NO ACTION ON DELETE CASCADE, CONSTRAINT "cocktails_ingredients_ingredient_id_fkey" FOREIGN KEY ("ingredient_id") REFERENCES "public"."ingredients" ("id") ON UPDATE NO ACTION ON DELETE CASCADE);
