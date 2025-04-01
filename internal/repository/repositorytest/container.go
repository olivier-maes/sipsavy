package repositorytest

import (
	"context"
	"fmt"
	"path/filepath"
	"time"

	"github.com/jackc/pgx/v5/pgxpool"
	"github.com/testcontainers/testcontainers-go"
	"github.com/testcontainers/testcontainers-go/modules/postgres"
	"github.com/testcontainers/testcontainers-go/wait"
)

const (
	dbName     = "sipsavy"
	dbUser     = "postgres"
	dbPassword = "postgres"
)

func CreatePostgresContainer(ctx context.Context) (*postgres.PostgresContainer, error) {
	return postgres.Run(ctx, "postgres:16-alpine",
		postgres.WithDatabase(dbName),
		postgres.WithUsername(dbUser),
		postgres.WithPassword(dbPassword),
		postgres.WithInitScripts(
			filepath.Join("repositorytest", "../../../sql/database.sql"),
			filepath.Join("repositorytest", "z01_fixtures.sql"),
		),
		testcontainers.WithWaitStrategy(
			wait.ForLog("database system is ready to accept connections").
				WithOccurrence(2).
				WithStartupTimeout(5*time.Second),
		),
	)
}

func StopPostgresContainer(container *postgres.PostgresContainer) error {
	return testcontainers.TerminateContainer(container, testcontainers.StopTimeout(5*time.Second))
}

func ConnectToDatabaseContainer(ctx context.Context, container *postgres.PostgresContainer) (*pgxpool.Pool, error) {
	dsn, err := container.ConnectionString(ctx)
	if err != nil {
		return nil, fmt.Errorf("failed to determine connection string: %w", err)
	}

	pool, err := pgxpool.New(ctx, dsn)
	if err != nil {
		return nil, fmt.Errorf("failed to connect to database: %w", err)
	}

	err = pool.Ping(ctx)
	if err != nil {
		return nil, fmt.Errorf("could not ping database: %w", err)
	}

	return pool, nil
}

func StartAndConnectToDatabaseContainer(ctx context.Context) (*pgxpool.Pool, func() error, error) {
	container, err := CreatePostgresContainer(ctx)
	if err != nil {
		return nil, nil, err
	}

	pool, err := ConnectToDatabaseContainer(ctx, container)
	if err != nil {
		return nil, nil, err
	}

	return pool, func() error {
		return StopPostgresContainer(container)
	}, nil
}
