package repository

import (
	"context"
	"fmt"

	"github.com/jackc/pgx/v5/pgxpool"

	"github.com/olivier-maes/sipsavy/internal"
	"github.com/olivier-maes/sipsavy/internal/repository/sql"
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

func (r CocktailRepository) GetById(ctx context.Context, id int) (internal.Cocktail, error) {
	getByIdQuery := sql.NewGetCocktailByIdQuery(id)
	row, err := getByIdQuery(ctx, r.conn)
	if err != nil {
		return internal.Cocktail{}, err
	}

	return row.ToCocktail(), nil
}

func (r CocktailRepository) GetLatest(ctx context.Context) ([]internal.Cocktail, error) {
	getLatestQuery := sql.NewGetLatestCocktailsQuery()
	rows, err := getLatestQuery(ctx, r.conn)
	if err != nil {
		return []internal.Cocktail{}, err
	}

	cocktails := make([]internal.Cocktail, len(rows))
	for i, row := range rows {
		c := row.ToCocktail()
		cocktails[i] = c
	}

	return cocktails, nil
}

func NewCocktailRepository(conn *pgxpool.Pool) CocktailRepository {
	return CocktailRepository{conn: conn}
}
