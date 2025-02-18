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
	tr template.Renderer,
	sm scs.SessionManager) *http.Server {
	mux := http.NewServeMux()

	// Static files
	mux.Handle("GET /static/", http.FileServerFS(ui.Files))

	// Dynamic middleware
	dynamic := alice.New(sm.LoadAndSave, noSurf, tr.Authenticate)

	// Home
	mux.Handle("GET /{$}", dynamic.ThenFunc(tr.Home))

	// User
	mux.Handle("GET /user/signup", dynamic.ThenFunc(tr.UserSignup))
	mux.Handle("POST /user/signup", dynamic.ThenFunc(tr.UserSignupPost))
	mux.Handle("GET /user/login", dynamic.ThenFunc(tr.UserLogin))
	mux.Handle("POST /user/login", dynamic.ThenFunc(tr.UserLoginPost))

	// Cocktail
	mux.Handle("GET /cocktail/{id}", dynamic.ThenFunc(tr.ViewCocktail))

	// PROTECTED ROUTES
	protected := dynamic.Append(requireAuthentication)

	// Cocktail
	mux.Handle("GET /cocktail/create", protected.ThenFunc(tr.CreateCocktail))
	mux.Handle("POST /cocktail", protected.ThenFunc(tr.CreateCocktailPost))

	// User
	mux.Handle("POST /user/logout", protected.ThenFunc(tr.UserLogoutPost))

	// Standard middleware
	standard := alice.New(tr.RecoverPanic, logRequest, commonHeaders)

	// TLS
	tlsConfig := &tls.Config{
		CurvePreferences: []tls.CurveID{tls.X25519, tls.CurveP256},
	}

	return &http.Server{
		Addr:         config.serverListenAddress,
		Handler:      standard.Then(mux),
		BaseContext:  func(net.Listener) context.Context { return context.WithoutCancel(parentCtx) },
		TLSConfig:    tlsConfig,
		IdleTimeout:  config.idleTimeout,
		ReadTimeout:  config.readHeaderTimeout,
		WriteTimeout: config.writeTimeout,
	}
}
