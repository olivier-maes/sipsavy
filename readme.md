# sipsavy

sipsavy is a web app for storing cocktail recipes.

## Development setup

```bash
docker compose up
```

### Atlas

[Altas](https://atlasgo.io/docs) is a tool for managing database migrations.

#### Usage

First of make the necessary changes to the postgres db. Then inspect the database (all schemas) and update
database.sql with the following command.

```bash 
atlas schema inspect -u "postgres://postgres:postgres@localhost:5432/sipsavy?sslmode=disable" --format '{{ sql . }}' > ./sql/database.sql
```

When the database.sql is up to date with all the necessary changes a new migration can be created
based on the diff in database.sql.

```bash
atlas migrate diff <<migration-name>> --dir "file://migrations" --to "file://sql/database.sql" --dev-url "docker://postgres/16/dev" 
```

Finally, the migration can be applied to the database with the following command.

```bash
atlas migrate apply --url "postgres://postgres:postgres@localhost:5432/sipsavy?sslmode=disable"
```

### TLS

To generate a self-signed TLS, locate the `generate_cert.go` file and run the following command, you should then place
the generated files in the `tls` directory under the root of the project.

```bash
go run /Users/<<username>>/go/go1.24.0/src/crypto/tls/generate_cert.go --rsa-bits=2048 --host=localhost
```