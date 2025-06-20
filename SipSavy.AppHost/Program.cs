var builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL
var postgres = builder
    .AddPostgres("postgres", port: 54320)
    .WithImage("ankane/pgvector")
    .WithImageTag("latest")
    .WithDataVolume("postgres-data")
    .WithLifetime(ContainerLifetime.Persistent);

var database = postgres.AddDatabase("sipsavy");

// Ollama
var ollama = builder.AddOllama("ollama")
    .WithDataVolume("ollama-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithGPUSupport();

// SipSavy Worker application
var worker = builder.AddProject<Projects.SipSavy_Worker>("sipsavy-worker")
    .WithReference(database)
    .WithReference(ollama)
    .WaitFor(postgres)
    .WaitFor(ollama)
    .WithEnvironment("YOUTUBE_CHANNEL_ID", "UCioZY1p0bZ4Xt-yodw8_cBQ")
    .WithEnvironment("AI_CHAT_MODEL", "llama3.1")
    .WithEnvironment("AI_EMBEDDING_MODEL", "all-minilm");

// SipSavy Web application
builder.AddProject<Projects.SipSavy_Web>("sipsavy-web")
    .WithReference(database)
    .WaitFor(postgres)
    .WaitFor(worker);

builder.Build().Run();