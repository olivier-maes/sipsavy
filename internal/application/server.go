package application

import (
	"context"
	"crypto/tls"
	"net"
	"net/http"
	"time"

	"github.com/oli4maes/sipsavy/ui"
)

func NewServer(
	parentCtx context.Context,
	address string,
	idleTimeout time.Duration,
	readTimeout time.Duration,
	writeTimeout time.Duration,
	tlsConfig *tls.Config) *http.Server {
	mux := http.NewServeMux()

	mux.Handle("GET /static/", http.FileServerFS(ui.Files))

	return &http.Server{
		Addr:         address,
		Handler:      mux,
		BaseContext:  func(net.Listener) context.Context { return context.WithoutCancel(parentCtx) },
		TLSConfig:    tlsConfig,
		IdleTimeout:  idleTimeout,
		ReadTimeout:  readTimeout,
		WriteTimeout: writeTimeout,
	}
}
