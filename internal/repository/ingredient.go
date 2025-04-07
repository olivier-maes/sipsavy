package repository

import (
	"context"
	"fmt"

	"github.com/jackc/pgx/v5/pgxpool"

	"github.com/olivier-maes/sipsavy/internal"
	"github.com/olivier-maes/sipsavy/internal/repository/sql"
)

type IngredientRepository struct {
	conn *pgxpool.Pool
}

func (r IngredientRepository) AddIngredient(ctx context.Context, ingredient internal.Ingredient) (int, error) {
	var id int
	var row sql.IngredientRow
	row.FromIngredient(ingredient)

	insertQuery := sql.NewInsertIngredientQuery(row)
	id, err := insertQuery(ctx, r.conn)
	if err != nil {
		return 0, fmt.Errorf("failed to add new ingredient: %w", err)
	}

	return id, nil
}

func (r IngredientRepository) ListIngredients(ctx context.Context) ([]internal.Ingredient, error) {
	listQuery := sql.NewListIngredientsQuery()
	rows, err := listQuery(ctx, r.conn)
	if err != nil {
		return []internal.Ingredient{}, err
	}

	ingredients := make([]internal.Ingredient, len(rows))
	for i, row := range rows {
		ing := row.ToIngredient()
		ingredients[i] = ing
	}

	return ingredients, nil
}

func NewIngredientRepository(conn *pgxpool.Pool) IngredientRepository {
	return IngredientRepository{conn: conn}
}
