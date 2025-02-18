package template

import "net/http"

func (tr Renderer) UserLogoutPost(w http.ResponseWriter, r *http.Request) {
	err := tr.sessionManager.RenewToken(r.Context())
	if err != nil {
		tr.serverError(w, r, err)
		return
	}

	tr.sessionManager.Remove(r.Context(), "authenticatedUserID")
	tr.sessionManager.Put(r.Context(), "flash", "You've been logged out.")

	http.Redirect(w, r, "/", http.StatusSeeOther)
}
