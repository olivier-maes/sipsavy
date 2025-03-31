package template

import (
	"errors"
	"net/http"

	errors2 "github.com/olivier-maes/sipsavy/internal"
)

type userSignupForm struct {
	Name     string `form:"name"`
	Email    string `form:"email"`
	Password string `form:"password"`
}

func (tr Renderer) UserSignup(w http.ResponseWriter, r *http.Request) {
	d := tr.newData(r)
	d.Form = userSignupForm{}
	tr.render(w, r, http.StatusOK, "signup.tmpl", d)
}

func (tr Renderer) UserSignupPost(w http.ResponseWriter, r *http.Request) {
	var f userSignupForm

	err := tr.decodePostForm(r, &f)
	if err != nil {
		tr.clientError(w, http.StatusBadRequest)
		return
	}

	err = tr.userRepo.AddUser(r.Context(), f.Name, f.Email, f.Password)
	if err != nil {
		if errors.Is(err, errors2.ErrDuplicateEmail) {
			d := tr.newData(r)
			d.Form = f
			tr.render(w, r, http.StatusUnprocessableEntity, "signup.tmpl", d)
		} else {
			tr.serverError(w, r, err)
		}
		return
	}

	tr.SessionManager.Put(r.Context(), "flash", "You've successfully signed up, please log in.")

	http.Redirect(w, r, "/user/login", http.StatusSeeOther)
}
