package repository

import (
	"context"
	"fmt"

	"github.com/jackc/pgx/v5/pgxpool"

	"github.com/oli4maes/sipsavy/internal"
	"github.com/oli4maes/sipsavy/internal/repository/sql"
)

type CocktailRepository struct {
	conn *pgxpool.Pool
}

func (r CocktailRepository) AddCocktail(ctx context.Context, cocktail internal.Cocktail) (int, error) {
	var id int
	var row sql.CocktailRow
	row.FromCocktail(cocktail)

	insertQuery := sql.NewInsertCocktailQuery(row)
	id, err := insertQuery(ctx, r.conn)
	if err != nil {
		return 0, fmt.Errorf("failed to add new cocktail: %w", err)
	}

	return id, nil
}

func NewCocktailRepository(conn *pgxpool.Pool) CocktailRepository {
	return CocktailRepository{conn: conn}
}
