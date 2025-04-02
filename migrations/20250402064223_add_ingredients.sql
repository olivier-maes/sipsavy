-- Create "ingredients" table
CREATE TABLE "public"."ingredients" ("id" serial NOT NULL, "name" text NOT NULL, PRIMARY KEY ("id"));
-- Create "cocktails_ingredients" table
CREATE TABLE "public"."cocktails_ingredients" ("cocktail_id" integer NOT NULL, "ingredient_id" integer NOT NULL, "amount" integer NOT NULL, "unit" text NOT NULL, PRIMARY KEY ("cocktail_id", "ingredient_id"), CONSTRAINT "cocktails_ingredients_cocktail_id_fkey" FOREIGN KEY ("cocktail_id") REFERENCES "public"."cocktails" ("id") ON UPDATE NO ACTION ON DELETE CASCADE, CONSTRAINT "cocktails_ingredients_ingredient_id_fkey" FOREIGN KEY ("ingredient_id") REFERENCES "public"."ingredients" ("id") ON UPDATE NO ACTION ON DELETE CASCADE);
