package template

import "net/http"

func (tr Renderer) UserLogoutPost(w http.ResponseWriter, r *http.Request) {
	err := tr.SessionManager.RenewToken(r.Context())
	if err != nil {
		tr.serverError(w, r, err)
		return
	}

	tr.SessionManager.Remove(r.Context(), "authenticatedUserID")
	tr.SessionManager.Put(r.Context(), "flash", "You've been logged out.")

	http.Redirect(w, r, "/", http.StatusSeeOther)
}
