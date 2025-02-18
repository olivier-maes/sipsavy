package template

import (
	"errors"
	"net/http"

	errors2 "github.com/oli4maes/sipsavy/internal"
)

type userLoginForm struct {
	Email    string `form:"email"`
	Password string `form:"password"`
}

func (tr Renderer) UserLogin(w http.ResponseWriter, r *http.Request) {
	d := tr.newData(r)
	d.Form = userLoginForm{}
	tr.render(w, r, http.StatusOK, "login.tmpl", d)
}

func (tr Renderer) UserLoginPost(w http.ResponseWriter, r *http.Request) {
	var f userLoginForm

	err := tr.decodePostForm(r, &f)
	if err != nil {
		tr.clientError(w, http.StatusBadRequest)
		return
	}

	id, err := tr.userRepo.Authenticate(r.Context(), f.Email, f.Password)
	if err != nil {
		if errors.Is(err, errors2.ErrInvalidCredentials) {
			d := tr.newData(r)
			d.Form = f
			tr.render(w, r, http.StatusUnprocessableEntity, "login.tmpl", d)
		} else {
			tr.clientError(w, http.StatusInternalServerError)
		}
	}

	err = tr.sessionManager.RenewToken(r.Context())
	if err != nil {
		tr.serverError(w, r, err)
		return
	}

	tr.sessionManager.Put(r.Context(), "authenticatedUserID", id)

	http.Redirect(w, r, "/cocktail/create", http.StatusSeeOther)
}
