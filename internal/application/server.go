package application

import (
	"context"
	"crypto/tls"
	"net"
	"net/http"

	"github.com/alexedwards/scs/v2"
	"github.com/justinas/alice"

	"github.com/oli4maes/sipsavy/internal/template"
	"github.com/oli4maes/sipsavy/ui"
)

func newServer(
	parentCtx context.Context,
	config Config,
	tlsConfig *tls.Config,
	templateRenderer template.Renderer,
	sessionManager scs.SessionManager) *http.Server {
	mux := http.NewServeMux()

	// Static files
	mux.Handle("GET /static/", http.FileServerFS(ui.Files))

	// Dynamic middleware
	dynamic := alice.New(sessionManager.LoadAndSave, noSurf)

	// Home
	mux.Handle("GET /{$}", dynamic.ThenFunc(templateRenderer.Home))

	// PROTECTED ROUTES
	//protected := dynamic.Append(requireAuthentication)

	// TODO: add protected middleware
	// Cocktail
	mux.Handle("GET /cocktail/create", dynamic.ThenFunc(templateRenderer.CreateCocktail))
	mux.Handle("POST /cocktail", dynamic.ThenFunc(templateRenderer.CreateCocktailPost))

	// Standard middleware
	alice.New(logRequest, commonHeaders).Then(mux)

	return &http.Server{
		Addr:         config.serverListenAddress,
		Handler:      mux,
		BaseContext:  func(net.Listener) context.Context { return context.WithoutCancel(parentCtx) },
		TLSConfig:    tlsConfig,
		IdleTimeout:  config.idleTimeout,
		ReadTimeout:  config.readHeaderTimeout,
		WriteTimeout: config.writeTimeout,
	}
}
