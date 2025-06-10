using SipSavy.Worker;
using SipSavy.ServiceDefaults;
using SipSavy.Worker.Data;
using SipSavy.Worker.Youtube.Features.ExtractTranscription;
using SipSavy.Worker.Youtube.Features.GetVideosByChannelId;

var builder = Host.CreateApplicationBuilder(args);

// Aspire service defaults
builder.AddServiceDefaults();

// Data
builder.AddNpgsqlDbContext<WorkerDbContext>("sipsavy-worker-db");

// Handlers (should be registered as singleton because the worker is a singleton)
builder.Services.AddSingleton<GetVideosByChannelIdHandler>();
builder.Services.AddSingleton<ExtractTranscriptionHandler>();

// Worker
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();