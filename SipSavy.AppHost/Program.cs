var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder
    .AddSqlServer("sql-server", port: 14329)
    .WithEndpoint(name: "sqlEndpoint", targetPort: 14330)
    .WithDataVolume("sql-server-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("sipsavy");

builder.AddProject<Projects.SipSavy_Web>("web")
    .WithReference(sqlServer)
    .WaitFor(sqlServer);

builder.AddProject<Projects.SipSavy_MigrationService>("migrations")
    .WithReference(sqlServer)
    .WaitFor(sqlServer);

builder.Build().Run();