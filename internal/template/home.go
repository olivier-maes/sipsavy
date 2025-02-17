package template

import "net/http"

func (tr Renderer) Home(w http.ResponseWriter, r *http.Request) {
	tr.render(w, r, http.StatusOK, "home.tmpl", tr.newData(r))
}
