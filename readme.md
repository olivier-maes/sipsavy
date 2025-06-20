# Sipsavy

## Description

Sipsavy is a web application that allows users to find cocktail recipes from a particular YouTube show.

### EF Core

#### Migration

To create a new migration, run the following command in the terminal:

```bash
dotnet ef migrations add <MigrationName> --project Sipsavy.Data --startup-project Sipsavy.Worker
```

Migrations will be applied automatically when the application starts.