package template

import (
	"net/http"
	"strconv"
)

func (tr Renderer) ViewCocktail(w http.ResponseWriter, r *http.Request) {
	id, err := strconv.Atoi(r.PathValue("id"))
	if err != nil || id < 1 {
		http.NotFound(w, r)
		return
	}

	cocktail, err := tr.cocktailRepo.GetById(r.Context(), id)
	if err != nil {
		tr.serverError(w, r, err)
		return
	}

	d := tr.newData(r)
	d.Cocktail = cocktail

	tr.render(w, r, http.StatusOK, "cocktail.tmpl", d)
}
