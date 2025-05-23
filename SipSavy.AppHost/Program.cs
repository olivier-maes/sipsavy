var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder
    .AddSqlServer("sql-server")
    .WithDataVolume("sql-server-data")
    .AddDatabase("sipsavy");

builder.AddProject<Projects.SipSavy_Web>("web")
    .WithReference(sqlServer)
    .WaitFor(sqlServer);

builder.Build().Run();