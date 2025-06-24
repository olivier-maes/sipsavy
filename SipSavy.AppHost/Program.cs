var builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL
var postgres = builder
    .AddPostgres("postgres", port: 54320)
    .WithImageTag("latest")
    .WithDataVolume("postgres-data")
    .WithLifetime(ContainerLifetime.Persistent);

var database = postgres.AddDatabase("sipsavy");

// Ollama
var ollama = builder.AddOllama("ollama")
    .WithDataVolume("ollama-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithGPUSupport();

// SipSavy Migration application
var migrationWorker = builder.AddProject<Projects.SipSavy_MigrationWorker>("sipsavy-migration-worker")
    .WithReference(database)
    .WaitFor(postgres);

// SipSavy Worker application
var worker = builder.AddProject<Projects.SipSavy_Worker>("sipsavy-worker")
    .WithReference(database)
    .WithReference(ollama)
    .WaitFor(postgres)
    .WaitFor(ollama)
    .WaitFor(migrationWorker)
    .WithEnvironment("YOUTUBE_CHANNEL_ID", "UCioZY1p0bZ4Xt-yodw8_cBQ")
    .WithEnvironment("AI_CHAT_MODEL", "llama3.1");

// SipSavy Web application
builder.AddProject<Projects.SipSavy_Web>("sipsavy-web")
    .WithReference(database)
    .WaitFor(postgres)
    .WaitFor(worker)
    .WaitFor(migrationWorker);

builder.Build().Run();