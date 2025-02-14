package application

import (
	"io"
	"log/slog"
)

func configureLogger(w io.Writer, level slog.Level) {
	opts := &slog.HandlerOptions{
		AddSource: false,
		Level:     level,
	}

	slog.SetDefault(slog.New(slog.NewTextHandler(w, opts)))
}
