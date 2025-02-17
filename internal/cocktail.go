package internal

import "time"

type Cocktail struct {
	id   int
	name string

	created time.Time
}

func (c *Cocktail) ID() int { return c.id }

func (c *Cocktail) Name() string { return c.name }

func (c *Cocktail) Created() time.Time { return c.created }

func NewCocktail(Id int, Name string, Created time.Time) Cocktail {
	return Cocktail{
		id:      Id,
		name:    Name,
		created: Created,
	}
}
