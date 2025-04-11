package internal

type CocktailIngredient struct {
	CocktailID   int
	IngredientID int
	Quantity     int
	Unit         Unit
}

func NewCocktailIngredient(cocktailID, ingredientID, quantity int, unit Unit) CocktailIngredient {
	return CocktailIngredient{
		CocktailID:   cocktailID,
		IngredientID: ingredientID,
		Quantity:     quantity,
		Unit:         unit,
	}
}
