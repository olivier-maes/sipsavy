package internal

import "time"

type Cocktail struct {
	ID   int
	Name string

	Created time.Time
}

func NewCocktail(ID int, Name string, Created time.Time) Cocktail {
	return Cocktail{
		ID:      ID,
		Name:    Name,
		Created: Created,
	}
}
