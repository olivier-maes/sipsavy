var builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL
var postgres = builder
    .AddPostgres("postgres")
    .WithImage("ankane/pgvector")
    .WithImageTag("latest")
    .WithDataVolume("postgres-data")
    .WithLifetime(ContainerLifetime.Persistent);

var webDb = postgres.AddDatabase("sipsavy-web-db");
var workerDb = postgres.AddDatabase("sipsavy-worker-db");

// Ollama
var ollama = builder.AddOllama("ollama")
    .WithDataVolume("ollama-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithGPUSupport();

var embeddings = ollama.AddModel("embedding", "all-minilm");
var chat = ollama.AddModel("chat", "llama3.1");

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
    .WithReference(embeddings)
    .WaitFor(postgres)
    .WaitFor(ollama)
    .WaitForCompletion(migrationService);

builder.Build().Run();