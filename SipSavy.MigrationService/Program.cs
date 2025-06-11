using SipSavy.Web.Data;
using SipSavy.MigrationService.Workers;
using SipSavy.ServiceDefaults;
using SipSavy.Worker.Data;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

// Workers
builder.Services.AddHostedService<WebMigrationWorker>();
builder.Services.AddHostedService<WorkerMigrationWorker>();

// OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(WebMigrationWorker.ActivitySourceName));

// Data
builder.AddNpgsqlDbContext<WebDbContext>("sipsavy-web-db");
builder.AddNpgsqlDbContext<WorkerDbContext>("sipsavy-worker-db");

var host = builder.Build();
host.Run();