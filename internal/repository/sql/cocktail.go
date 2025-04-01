package sql

import (
	"context"
	"errors"
	"time"

	"github.com/jackc/pgx/v5"

	"github.com/olivier-maes/sipsavy/internal"
)

type CocktailRow struct {
	ID      int
	Name    string
	Created time.Time
}

func (r *CocktailRow) FromCocktail(c internal.Cocktail) {
	r.ID = c.ID
	r.Name = c.Name
	r.Created = c.Created
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

const getCocktailRowByID = `SELECT id, name, created FROM cocktails WHERE id = $1`

func NewGetCocktailByIDQuery(id int) QueryRow[CocktailRow] {
	return func(ctx context.Context, conn Connection) (CocktailRow, error) {
		args := []any{
			id,
		}

		var row CocktailRow
		err := conn.QueryRow(ctx, getCocktailRowByID, args...).Scan(&row.ID, &row.Name, &row.Created)
		if err != nil {
			if errors.Is(err, pgx.ErrNoRows) {
				return CocktailRow{}, internal.ErrNoRecord
			}
			return CocktailRow{}, err
		}
		return row, nil
	}
}

const getLatestCocktailsRow = `SELECT id, name, created 
								FROM cocktails 
								ORDER BY created DESC limit 10`

func NewGetLatestCocktailsQuery() Query[CocktailRow] {
	return func(ctx context.Context, conn Connection) ([]CocktailRow, error) {
		rows, err := conn.Query(ctx, getLatestCocktailsRow)
		if err != nil {
			return []CocktailRow{}, err
		}

		cocktailRows, err := pgx.CollectRows(rows, pgx.RowToStructByName[CocktailRow])
		if err != nil {
			return []CocktailRow{}, err
		}

		return cocktailRows, nil
	}
}
