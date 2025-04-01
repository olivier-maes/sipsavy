package repository

import (
	"context"
	"testing"
	"time"

	"github.com/olivier-maes/sipsavy/internal"
	"github.com/olivier-maes/sipsavy/internal/repository/repositorytest"
)

func TestCocktailRepository_AddCocktail(t *testing.T) {
	ctx := context.Background()
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

	addedTime, err := time.Parse(time.RFC3339, "2006-01-02T15:04:05Z")
	if err != nil {
		t.Fatal(err)
	}

	cocktailToAdd := internal.NewCocktail(0, "testcocktail", addedTime)

	repo := NewCocktailRepository(conn)

	_, err = repo.AddCocktail(ctx, cocktailToAdd)
	if err != nil {
		t.Errorf("error adding document: %s", err)
	}
}
