var builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL
var postgres = builder
    .AddPostgres("postgres", port: 5432)
    .WithEndpoint(targetPort: 5432, name: "postgres-endpoint")
    .WithDataVolume("postgres-data")
    .WithLifetime(ContainerLifetime.Persistent);

var webDb = postgres.AddDatabase("sipsavy-web-db");
var workerDb = postgres.AddDatabase("sipsavy-worker-db");

// Migration Service
var migrationService = builder.AddProject<Projects.SipSavy_MigrationService>("migration-service")
    .WithReference(webDb)
    .WithReference(workerDb)
    .WaitFor(postgres);

// SipSavy Web application
builder.AddProject<Projects.SipSavy_Web>("sipsavy-web")
    .WithReference(webDb)
    .WaitFor(postgres)
    .WaitForCompletion(migrationService);

// SipSavy Worker application
builder.AddProject<Projects.SipSavy_Worker>("sipsavy-worker")
    .WithReference(workerDb)
    .WaitFor(postgres)
    .WaitForCompletion(migrationService);

builder.Build().Run();