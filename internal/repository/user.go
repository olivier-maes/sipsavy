package repository

import (
	"context"
	"errors"
	"fmt"

	"github.com/jackc/pgx/v5/pgxpool"
	"golang.org/x/crypto/bcrypt"

	errors2 "github.com/oli4maes/sipsavy/internal/errors"
	"github.com/oli4maes/sipsavy/internal/repository/sql"
)

type UserRepository struct {
	conn *pgxpool.Pool
}

func (r UserRepository) AddUser(ctx context.Context, name string, email string, password string) error {
	hashedPassword, err := bcrypt.GenerateFromPassword([]byte(password), bcrypt.DefaultCost)
	if err != nil {
		return err
	}

	insertQuery := sql.NewInsertUserQuery(sql.UserRow{
		Name:           name,
		Email:          email,
		HashedPassword: hashedPassword,
	})

	err = insertQuery(ctx, r.conn)
	if err != nil {
		return fmt.Errorf("failed to insert new user: %w", err)
	}
	return nil
}

func (r UserRepository) Authenticate(ctx context.Context, email string, password string) (int, error) {
	authenticateQuery := sql.NewAuthenticateUserQuery(email)

	row, err := authenticateQuery(ctx, r.conn)
	if err != nil {
		return 0, err
	}

	err = bcrypt.CompareHashAndPassword(row.HashedPassword, []byte(password))
	if err != nil {
		if errors.Is(err, bcrypt.ErrMismatchedHashAndPassword) {
			return 0, errors2.ErrInvalidCredentials
		} else {
			return 0, err
		}
	}

	return row.ID, nil
}

func (r UserRepository) Exists(ctx context.Context, id int) (bool, error) {
	existsQuery := sql.NewUserExistsQuery(id)

	exists, err := existsQuery(ctx, r.conn)
	if err != nil {
		return false, err
	}

	return exists, nil
}

func NewUserRepository(conn *pgxpool.Pool) UserRepository {
	return UserRepository{conn: conn}
}
