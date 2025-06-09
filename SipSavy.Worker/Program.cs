using SipSavy.Worker;
using SipSavy.Worker.Youtube.Features.GetVideosByChannelId;

var builder = Host.CreateApplicationBuilder(args);

// Handlers (should be registered as singleton because the worker is a singleton)
builder.Services.AddSingleton<GetVideosByChannelIdHandler>();

// Worker
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();