package main

import (
	"context"
	"os"

	"github.com/oli4maes/sipsavy/internal/application"
)

func main() {
	ctx := context.Background()
	config := application.NewConfig(os.LookupEnv)
	if err := application.Run(ctx, config); err != nil {
		os.Exit(1)
	}
}
