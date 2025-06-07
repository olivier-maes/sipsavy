var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder
    .AddPostgres("postgres", port: 5432)
    .WithEndpoint(targetPort: 5432, name: "postgres-endpoint")
    .WithDataVolume("postgres-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("sipsavy");

builder.AddProject<Projects.SipSavy_Web>("web")
    .WithReference(postgres)
    .WaitFor(postgres);

builder.AddProject<Projects.SipSavy_MigrationService>("migration-service")
    .WithReference(postgres)
    .WaitFor(postgres);

builder.Build().Run();