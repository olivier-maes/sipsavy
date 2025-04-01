package template

import (
	"bytes"
	"context"
	"errors"
	"fmt"
	"html/template"
	"io/fs"
	"log/slog"
	"net/http"
	"path/filepath"
	"time"

	"github.com/alexedwards/scs/v2"
	f "github.com/go-playground/form/v4"
	"github.com/justinas/nosurf"

	"github.com/olivier-maes/sipsavy/internal"
	"github.com/olivier-maes/sipsavy/ui"
)

type contextKey string

const isAuthenticatedContextKey = contextKey("isAuthenticated")

type cocktailRepo interface {
	AddCocktail(ctx context.Context, cocktail internal.Cocktail) (int, error)
	GetByID(ctx context.Context, id int) (internal.Cocktail, error)
	GetLatest(ctx context.Context) ([]internal.Cocktail, error)
}

type userRepo interface {
	AddUser(ctx context.Context, name string, email string, password string) error
	Authenticate(ctx context.Context, email string, password string) (int, error)
	Exists(ctx context.Context, id int) (bool, error)
}

type Renderer struct {
	templateCache  map[string]*template.Template
	formDecoder    *f.Decoder
	SessionManager *scs.SessionManager
	cocktailRepo   cocktailRepo
	userRepo       userRepo
}

func NewRenderer(cocktailRepo cocktailRepo, userRepo userRepo, sessionManager *scs.SessionManager) (Renderer, error) {
	templateCache, err := newCache()
	if err != nil {
		return Renderer{}, err
	}

	return Renderer{
		templateCache:  templateCache,
		formDecoder:    f.NewDecoder(),
		cocktailRepo:   cocktailRepo,
		userRepo:       userRepo,
		SessionManager: sessionManager,
	}, nil
}

func IsAuthenticated(r *http.Request) bool {
	authenticated, ok := r.Context().Value(isAuthenticatedContextKey).(bool)
	if !ok {
		return false
	}
	return authenticated
}

func (tr Renderer) Authenticate(next http.Handler) http.Handler {
	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		id := tr.SessionManager.GetInt(r.Context(), "authenticatedUserID")
		if id == 0 {
			next.ServeHTTP(w, r)
			return
		}

		exists, err := tr.userRepo.Exists(r.Context(), id)
		if err != nil {
			tr.serverError(w, r, err)
			return
		}

		if exists {
			ctx := context.WithValue(r.Context(), isAuthenticatedContextKey, true)
			r = r.WithContext(ctx)
		}

		next.ServeHTTP(w, r)
	})
}

func (tr Renderer) RecoverPanic(next http.Handler) http.Handler {
	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		defer func() {
			if err := recover(); err != nil {
				w.Header().Set("Connection", "close")
				tr.serverError(w, r, fmt.Errorf("%s", err))
			}
		}()

		next.ServeHTTP(w, r)
	})
}

type data struct {
	IsAuthenticated bool
	CSRFToken       string
	Form            any
	Flash           string
	Cocktail        internal.Cocktail
	Cocktails       []internal.Cocktail
}

func (tr Renderer) newData(r *http.Request) data {
	return data{
		IsAuthenticated: IsAuthenticated(r),
		CSRFToken:       nosurf.Token(r),
	}
}

func humanDate(t time.Time) string {
	if t.IsZero() {
		return ""
	}

	return t.UTC().Format("02 Jan 2006")
}

func (tr Renderer) render(w http.ResponseWriter, r *http.Request, status int, page string, data data) {
	ts, ok := tr.templateCache[page]
	if !ok {
		err := fmt.Errorf("the template %s does not exist", page)
		tr.serverError(w, r, err)
		return
	}

	buf := new(bytes.Buffer)

	err := ts.ExecuteTemplate(buf, "base", data)
	if err != nil {
		tr.serverError(w, r, err)
		return
	}

	w.WriteHeader(status)

	_, err = buf.WriteTo(w)
	if err != nil {
		tr.serverError(w, r, err)
		return
	}
}

func (tr Renderer) serverError(w http.ResponseWriter, r *http.Request, err error) {
	var (
		method = r.Method
		uri    = r.URL.RequestURI()
	)

	slog.ErrorContext(r.Context(), err.Error(), "method", method, "uri", uri)
	http.Error(w, http.StatusText(http.StatusInternalServerError), http.StatusInternalServerError)
}

func (tr Renderer) clientError(w http.ResponseWriter, status int) {
	http.Error(w, http.StatusText(status), status)
}

func newCache() (map[string]*template.Template, error) {
	cache := map[string]*template.Template{}

	pages, err := fs.Glob(ui.Files, "html/pages/*.tmpl")
	if err != nil {
		return nil, err
	}

	for _, page := range pages {
		name := filepath.Base(page)

		patterns := []string{
			"html/base.tmpl",
			"html/partials/*.tmpl",
			page,
		}

		ts, templErr := template.New(name).Funcs(template.FuncMap{
			"humanDate": humanDate,
		}).ParseFS(ui.Files, patterns...)
		if templErr != nil {
			return nil, templErr
		}

		cache[name] = ts
	}

	return cache, nil
}

func (tr Renderer) decodePostForm(r *http.Request, dst any) error {
	err := r.ParseForm()
	if err != nil {
		return err
	}

	err = tr.formDecoder.Decode(dst, r.PostForm)
	if err != nil {
		var invalidDecoderError *f.InvalidDecoderError

		if errors.As(err, &invalidDecoderError) {
			panic(err)
		}

		return err
	}

	return nil
}
