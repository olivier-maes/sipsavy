package repository

import (
	"testing"

	"github.com/olivier-maes/sipsavy/internal"
	"github.com/olivier-maes/sipsavy/internal/repository/repositorytest"
)

func TestIngredientRepository_AddIngredient(t *testing.T) {
	ctx := t.Context()
	container, err := repositorytest.CreatePostgresContainer(ctx)
	if err != nil {
		t.Fatal(err)
	}

	t.Cleanup(func() {
		stopErr := repositorytest.StopPostgresContainer(container)
		if stopErr != nil {
			t.Error(stopErr)
		}
	})

	conn, err := repositorytest.ConnectToDatabaseContainer(ctx, container)
	if err != nil {
		t.Fatal(err)
	}

	ingredientToAdd := internal.NewIngredient(0, "testingredient")

	repo := NewIngredientRepository(conn)

	_, err = repo.AddIngredient(ctx, ingredientToAdd)
	if err != nil {
		t.Errorf("error adding document: %s", err)
	}
}
