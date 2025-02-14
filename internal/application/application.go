package application

import (
	"context"
	"crypto/tls"
	"errors"
	"fmt"
	"html/template"
	"log/slog"
	"net/http"
	"os"
	"os/signal"
	"time"

	"github.com/alexedwards/scs/v2"
	"github.com/go-playground/form/v4"
	"github.com/jackc/pgx/v5/pgxpool"
)

type application struct {
	dbConn         *pgxpool.Pool
	templateCache  map[string]*template.Template
	formDecoder    *form.Decoder
	sessionManager *scs.SessionManager
	server         *http.Server
}

func (a *application) start(ctx context.Context) (func(ctx2 context.Context) error, error) {
	signalCtx, stop := signal.NotifyContext(ctx, os.Interrupt, os.Kill)
	defer stop()

	errChan := make(chan error, 1)
	go func() {
		slog.InfoContext(ctx, fmt.Sprintf("Server listening on %s", a.server.Addr))
		if err := a.server.ListenAndServe(); err != nil && !errors.Is(err, http.ErrServerClosed) {
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
	// Initialize a database connection
	dbConn, err := initDatabaseConnection(ctx, config.databaseURL)
	if err != nil {
		return nil, fmt.Errorf("could not establish a database connection: %w", err)
	}

	tlsConfig := &tls.Config{
		CurvePreferences: []tls.CurveID{tls.X25519, tls.CurveP256},
	}

	// HTTP Server
	server := NewServer(ctx, config.serverListenAddress, config.idleTimeout, config.readHeaderTimeout, config.writeTimeout, tlsConfig)

	return &application{
		dbConn:         dbConn,
		templateCache:  nil,
		formDecoder:    nil,
		sessionManager: nil,
		server:         server,
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
