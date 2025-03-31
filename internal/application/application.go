package application

import (
	"context"
	"errors"
	"fmt"
	"log/slog"
	"net/http"
	"os"
	"os/signal"
	"time"

	"github.com/alexedwards/scs/pgxstore"
	"github.com/alexedwards/scs/v2"
	"github.com/jackc/pgx/v5/pgxpool"

	"github.com/olivier-maes/sipsavy/internal/repository"
	"github.com/olivier-maes/sipsavy/internal/template"
)

type application struct {
	dbConn *pgxpool.Pool
	server *http.Server
}

func (a *application) start(ctx context.Context) (func(ctx2 context.Context) error, error) {
	signalCtx, stop := signal.NotifyContext(ctx, os.Interrupt, os.Kill)
	defer stop()

	errChan := make(chan error, 1)
	go func() {
		slog.InfoContext(ctx, fmt.Sprintf("Server listening on %s", a.server.Addr))
		if err := a.server.ListenAndServeTLS("./tls/cert.pem", "./tls/key.pem"); err != nil && !errors.Is(err, http.ErrServerClosed) {
			errChan <- fmt.Errorf("failed to start server: %w", err)
		}
	}()

	select {
	case <-signalCtx.Done():
		slog.InfoContext(signalCtx, "Received shutdown signal, shutting down server")
	case err := <-errChan:
		return nil, err
	}

	return func(context.Context) error {
		shutdownCtx, cancel := context.WithTimeout(ctx, 5*time.Second)
		defer cancel()

		a.dbConn.Close()
		serverShutdownErr := a.server.Shutdown(shutdownCtx)

		return errors.Join(serverShutdownErr)
	}, nil
}

func newApplication(ctx context.Context, config Config) (*application, error) {
	// Database and repositories
	dbConn, err := initDatabaseConnection(ctx, config.databaseURL)
	if err != nil {
		return nil, fmt.Errorf("could not establish a database connection: %w", err)
	}

	cocktailRepo := repository.NewCocktailRepository(dbConn)
	userRepo := repository.NewUserRepository(dbConn)

	// Session manager
	sessionManager := scs.New()
	sessionManager.Store = pgxstore.New(dbConn)
	sessionManager.Lifetime = config.sessionManagerLifetime

	// Templates
	templateRenderer, err := template.NewRenderer(cocktailRepo, userRepo, sessionManager)
	if err != nil {
		return nil, fmt.Errorf("could not create a template renderer: %w", err)
	}

	// Server
	server := newServer(ctx, config, templateRenderer)

	return &application{
		dbConn: dbConn,
		server: server,
	}, nil
}

func initDatabaseConnection(ctx context.Context, dbURL string) (*pgxpool.Pool, error) {
	dbConn, err := pgxpool.New(ctx, dbURL)
	if err != nil {
		return nil, err
	}

	err = dbConn.Ping(ctx)
	if err != nil {
		return nil, err
	}

	return dbConn, nil
}
