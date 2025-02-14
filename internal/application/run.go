package application

import (
	"context"
	"fmt"
	"log/slog"
	"os"
)

func Run(ctx context.Context, config Config) error {
	configureLogger(os.Stdout, config.logLevel)
	slog.InfoContext(ctx, fmt.Sprintf("Log level has been set to %s", config.logLevel.String()))

	app, err := newApplication(ctx, config)
	if err != nil {
		slog.ErrorContext(ctx, "Error initializing application", slog.Any("err", err))
		return err
	}

	stop, err := app.start(ctx)
	if err != nil {
		slog.ErrorContext(ctx, "Error starting application", slog.Any("err", err))
		return err
	}

	if err = stop(ctx); err != nil {
		slog.ErrorContext(ctx, "Could not gracefully stop application", slog.Any("err", err))
		return err
	}

	slog.InfoContext(ctx, "Shutdown complete")

	return nil
}
