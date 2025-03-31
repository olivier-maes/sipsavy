package sql

import (
	"context"
	"errors"
	"strings"
	"time"

	"github.com/jackc/pgx/v5"
	"github.com/jackc/pgx/v5/pgconn"

	errors2 "github.com/olivier-maes/sipsavy/internal"
)

type UserRow struct {
	ID             int
	Name           string
	Email          string
	HashedPassword []byte
	Created        time.Time
}

const insertUserRow = `INSERT INTO users (name, email, hashed_password, created) 
						VALUES ($1, $2, $3, current_timestamp)`

func NewInsertUserQuery(row UserRow) ExecQuery {
	return func(ctx context.Context, conn Connection) error {
		args := []any{
			row.Name,
			row.Email,
			row.HashedPassword,
		}

		_, err := conn.Exec(ctx, insertUserRow, args...)
		if err != nil {
			var pgErr *pgconn.PgError
			if errors.As(err, &pgErr) {
				if pgErr.Code == "23505" && strings.Contains(strings.ToLower(pgErr.Message), "users_uc_email") {
					return errors2.ErrDuplicateEmail
				}
			}
		}
		return err
	}
}

const authenticateUser = `SELECT id, hashed_password
							FROM users 
							WHERE email = $1`

func NewAuthenticateUserQuery(email string) QueryRow[UserRow] {
	return func(ctx context.Context, conn Connection) (UserRow, error) {
		row := UserRow{}

		err := conn.QueryRow(ctx, authenticateUser, email).Scan(&row.ID, &row.HashedPassword)
		if err != nil {
			if errors.Is(err, pgx.ErrNoRows) {
				return UserRow{}, errors2.ErrInvalidCredentials
			} else {
				return UserRow{}, err
			}
		}

		return row, nil
	}
}

const userExists = `SELECT EXISTS(SELECT 1 FROM users WHERE id = $1)`

func NewUserExistsQuery(id int) QueryRow[bool] {
	return func(ctx context.Context, conn Connection) (bool, error) {
		var exists bool

		err := conn.QueryRow(ctx, userExists, id).Scan(&exists)
		return exists, err
	}
}
