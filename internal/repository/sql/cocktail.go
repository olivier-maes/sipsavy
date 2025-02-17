package sql

import (
	"context"
	"time"

	"github.com/oli4maes/sipsavy/internal"
)

type CocktailRow struct {
	ID      int
	Name    string
	Created time.Time
}

func (r *CocktailRow) FromCocktail(c internal.Cocktail) {
	r.ID = c.ID()
	r.Name = c.Name()
	r.Created = c.Created()
}

func (r *CocktailRow) ToCocktail() internal.Cocktail {
	return internal.NewCocktail(r.ID, r.Name, r.Created)
}

const insertCocktailRow = `INSERT INTO cocktails (name, created) 
							VALUES ($1, $2)
							RETURNING id`

func NewInsertCocktailQuery(row CocktailRow) QueryRow[int] {
	return func(ctx context.Context, conn Connection) (int, error) {
		args := []any{
			row.Name,
			row.Created,
		}

		var id int
		err := conn.QueryRow(ctx, insertCocktailRow, args...).Scan(&id)
		if err != nil {
			return 0, err
		}
		return id, nil
	}
}
