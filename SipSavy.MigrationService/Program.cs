using SipSavy.Web.Data;
using SipSavy.MigrationService.Workers;
using SipSavy.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<WebMigrationWorker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(WebMigrationWorker.ActivitySourceName));

builder.AddNpgsqlDbContext<WebDbContext>("sipsavy-web-db");

var host = builder.Build();
host.Run();