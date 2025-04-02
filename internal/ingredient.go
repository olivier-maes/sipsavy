package internal

type Ingredient struct {
	ID   int
	Name string

	CocktailIngredients []CocktailIngredient
}

func NewIngredient(id int, name string) Ingredient {
	return Ingredient{
		ID:   id,
		Name: name,
	}
}
