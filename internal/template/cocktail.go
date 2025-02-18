package template

import (
	"errors"
	"net/http"
	"strconv"

	errors2 "github.com/oli4maes/sipsavy/internal/errors"
)

func (tr Renderer) ViewCocktail(w http.ResponseWriter, r *http.Request) {
	id, err := strconv.Atoi(r.PathValue("id"))
	if err != nil || id < 1 {
		http.NotFound(w, r)
		return
	}

	cocktail, err := tr.cocktailRepo.GetById(r.Context(), id)
	if err != nil {
		if errors.Is(err, errors2.ErrNoRecord) {
			http.NotFound(w, r)
		} else {
			tr.serverError(w, r, err)
		}
		return
	}

	d := tr.newData(r)
	d.Cocktail = cocktail

	tr.render(w, r, http.StatusOK, "cocktail.tmpl", d)
}
