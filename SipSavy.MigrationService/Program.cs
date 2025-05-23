using SipSavy.Data;
using SipSavy.MigrationService;
using SipSavy.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddSqlServerDbContext<AppDbContext>("sipsavy");

var host = builder.Build();
host.Run();
