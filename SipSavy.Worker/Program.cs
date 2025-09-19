using SipSavy.Data;
using SipSavy.ServiceDefaults;
using SipSavy.Data.Repository;
using SipSavy.Worker.Workers;
using YoutubeExplode;

var builder = Host.CreateApplicationBuilder(args);

// Aspire service defaults
builder.AddServiceDefaults();

// Ollama
builder.AddOllamaApiClient("ollama");

// Data
builder.AddNpgsqlDbContext<AppDbContext>("sipsavy");

builder.Services.AddScoped<IQueryFacade, QueryFacade>();
builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddScoped<ICocktailRepository, CocktailRepository>();

// YoutubeExplode
builder.Services.AddScoped<YoutubeClient>();

// Mediator
builder.Services.AddMediator();

// Worker
builder.Services.AddHostedService<TranscriptionWorker>();
builder.Services.AddHostedService<CocktailExtractionWorker>();

var host = builder.Build();
host.Run();