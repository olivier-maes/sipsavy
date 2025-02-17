package sql

import (
	"context"
	"errors"
	"log/slog"

	"github.com/jackc/pgx/v5"
	"github.com/jackc/pgx/v5/pgconn"
)

type Connection interface {
	Exec(context.Context, string, ...any) (pgconn.CommandTag, error)
	Query(context.Context, string, ...any) (pgx.Rows, error)
	QueryRow(context.Context, string, ...any) pgx.Row
}

// ExecQuery is a query that returns no result. It's typically used for update or delete statements.
type ExecQuery func(ctx context.Context, conn Connection) error

// Query is your typical SQL query: it returns zero or more rows.
type Query[T any] func(ctx context.Context, conn Connection) ([]T, error)

// QueryRow is a SQL query that returns a single row.
type QueryRow[T any] func(ctx context.Context, conn Connection) (T, error)

// CountQuery is a SQL query that returns the amount of rows.
type CountQuery func(ctx context.Context, conn Connection) (int, error)

// ScalarQuery is a SQL query that returns one column for one row.
type ScalarQuery[T any] func(ctx context.Context, conn Connection) (T, error)

// RollbackTx is a helper function to easily defer a tx rollback.
func RollbackTx(ctx context.Context, tx pgx.Tx) {
	rollbackErr := tx.Rollback(ctx)
	if rollbackErr != nil && !errors.Is(rollbackErr, pgx.ErrTxClosed) {
		slog.ErrorContext(ctx, "Failed to rollback transaction", slog.Any("err", rollbackErr))
	}
}
