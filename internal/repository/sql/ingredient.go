package sql

import (
	"context"

	"github.com/jackc/pgx/v5"

	"github.com/olivier-maes/sipsavy/internal"
)

type IngredientRow struct {
	ID   int
	Name string
}

func (r *IngredientRow) FromIngredient(i internal.Ingredient) {
	r.ID = i.ID
	r.Name = i.Name
}

func (r *IngredientRow) ToIngredient() internal.Ingredient {
	return internal.NewIngredient(r.ID, r.Name)
}

const insertIngredientRow = `INSERT INTO ingredients (name)
								VALUES ($1)
								RETURNING id`

func NewInsertIngredientQuery(row IngredientRow) QueryRow[int] {
	return func(ctx context.Context, conn Connection) (int, error) {
		args := []any{
			row.Name,
		}

		var id int
		err := conn.QueryRow(ctx, insertIngredientRow, args...).Scan(&id)
		if err != nil {
			return 0, err
		}
		return id, nil
	}
}

const listIngredientsRow = `SELECT id, name FROM ingredients
							ORDER BY name`

func NewListIngredientsQuery() QueryRow[[]IngredientRow] {
	return func(ctx context.Context, conn Connection) ([]IngredientRow, error) {
		rows, err := conn.Query(ctx, listIngredientsRow)
		if err != nil {
			return nil, err
		}
		defer rows.Close()

		ingredientRows, err := pgx.CollectRows(rows, pgx.RowToStructByName[IngredientRow])
		if err != nil {
			return []IngredientRow{}, err
		}

		return ingredientRows, nil
	}
}
