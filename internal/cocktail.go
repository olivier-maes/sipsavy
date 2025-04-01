package internal

import "time"

type Cocktail struct {
	ID   int
	Name string

	Created time.Time
}

func NewCocktail(id int, name string, created time.Time) Cocktail {
	return Cocktail{
		ID:      id,
		Name:    name,
		Created: created,
	}
}
