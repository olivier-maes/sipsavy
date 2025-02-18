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

	"github.com/oli4maes/sipsavy/internal"
	"github.com/oli4maes/sipsavy/ui"
)

type contextKey string

const isAuthenticatedContextKey = contextKey("isAuthenticated")

type cocktailRepo interface {
	AddCocktail(ctx context.Context, cocktail internal.Cocktail) (int, error)
	GetById(ctx context.Context, id int) (internal.Cocktail, error)
}

type Renderer struct {
	templateCache  map[string]*template.Template
	formDecoder    *f.Decoder
	sessionManager *scs.SessionManager
	cocktailRepo   cocktailRepo
}

func NewRenderer(cocktailRepo cocktailRepo, sessionManager *scs.SessionManager) (Renderer, error) {
	templateCache, err := newCache()
	if err != nil {
		return Renderer{}, err
	}

	return Renderer{
		templateCache:  templateCache,
		formDecoder:    f.NewDecoder(),
		cocktailRepo:   cocktailRepo,
		sessionManager: sessionManager,
	}, nil
}

func IsAuthenticated(r *http.Request) bool {
	authenticated, ok := r.Context().Value(isAuthenticatedContextKey).(bool)
	if !ok {
		return false
	}
	return authenticated
}

type data struct {
	IsAuthenticated bool
	CSRFToken       string
	Form            any
	Flash           string
	Cocktail        internal.Cocktail
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

	return t.UTC().Format("02 Jan 2006 at 15:04")
}

var functions = template.FuncMap{
	"humanDate": humanDate,
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

		ts, err := template.New(name).Funcs(functions).ParseFS(ui.Files, patterns...)
		if err != nil {
			return nil, err
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
