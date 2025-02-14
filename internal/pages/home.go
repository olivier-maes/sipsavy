package pages

import "net/http"

func (t TemplateRenderer) Home(w http.ResponseWriter, r *http.Request) {
	t.render(w, r, http.StatusOK, "home.tmpl", t.newTemplateData(r))
}
