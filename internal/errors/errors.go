package errors

import "errors"

var (
	ErrNoRecord           = errors.New("repository: no matching record found")
	ErrInvalidCredentials = errors.New("repository: invalid credentials")
	ErrDuplicateEmail     = errors.New("repository: duplicate email")
)
