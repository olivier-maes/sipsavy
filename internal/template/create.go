package template

import (
	"fmt"
	"net/http"
	"time"

	"github.com/olivier-maes/sipsavy/internal"
)

type form struct {
	Name string `form:"name"`
}

func (tr Renderer) CreateCocktail(w http.ResponseWriter, r *http.Request) {
	ingredients, err := tr.ingredientRepo.ListIngredients(r.Context())
	if err != nil {
		tr.serverError(w, r, err)
		return
	}

	d := tr.newData(r)
	d.Ingredients = ingredients
	d.Units = []internal.Unit{internal.UnitMilliliter, internal.UnitBarspoon}

	d.Form = form{}
	tr.render(w, r, http.StatusOK, "create.tmpl", d)
}

func (tr Renderer) CreateCocktailPost(w http.ResponseWriter, r *http.Request) {
	var f form

	err := tr.decodePostForm(r, &f)
	if err != nil {
		tr.clientError(w, http.StatusBadRequest)
		return
	}

	id, err := tr.cocktailRepo.AddCocktail(r.Context(), internal.NewCocktail(0, f.Name, time.Now()))
	if err != nil {
		tr.serverError(w, r, err)
		return
	}

	tr.SessionManager.Put(r.Context(), "flash", "Cocktail successfully created")

	http.Redirect(w, r, fmt.Sprintf("/cocktail/%d", id), http.StatusSeeOther)
}
