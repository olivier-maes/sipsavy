package template

import "net/http"

func (tr Renderer) Home(w http.ResponseWriter, r *http.Request) {
	cocktails, err := tr.cocktailRepo.GetLatest(r.Context())
	if err != nil {
		tr.serverError(w, r, err)
		return
	}

	d := tr.newData(r)
	d.Cocktails = cocktails

	tr.render(w, r, http.StatusOK, "home.tmpl", d)
}
