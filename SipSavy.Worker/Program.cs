using SipSavy.Data;
using SipSavy.ServiceDefaults;
using SipSavy.Data.Repository;
using SipSavy.Worker.Features.Cocktail.AddNewCocktails;
using SipSavy.Worker.Features.Cocktail.ExtractCocktails;
using SipSavy.Worker.Features.Embedding.GetEmbeddings;
using SipSavy.Worker.Features.Video.AddNewVideos;
using SipSavy.Worker.Features.Video.GetVideosByStatus;
using SipSavy.Worker.Features.Video.UpdateVideo;
using SipSavy.Worker.Features.Youtube.ExtractTranscription;
using SipSavy.Worker.Features.Youtube.GetVideosByChannelId;
using SipSavy.Worker.Workers;
using YoutubeExplode;

var builder = Host.CreateApplicationBuilder(args);

// Aspire service defaults
builder.AddServiceDefaults();

// Ollama
builder.AddOllamaApiClient("ollama").AddEmbeddingGenerator();

// Data
builder.AddNpgsqlDbContext<AppDbContext>("sipsavy");

builder.Services.AddScoped<IQueryFacade, QueryFacade>();
builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddScoped<ICocktailRepository, CocktailRepository>();

// YoutubeExplode
builder.Services.AddScoped<YoutubeClient>();

// Handlers
builder.Services.AddScoped<GetVideosByChannelIdHandler>();
builder.Services.AddScoped<ExtractTranscriptionHandler>();
builder.Services.AddScoped<AddNewVideosHandler>();
builder.Services.AddScoped<GetVideosByStatusHandler>();
builder.Services.AddScoped<UpdateVideoHandler>();
builder.Services.AddScoped<GetEmbeddingsHandler>();
builder.Services.AddScoped<ExtractCocktailsHandler>();
builder.Services.AddScoped<AddNewCocktailsHandler>();

// Worker
builder.Services.AddHostedService<TranscriptionWorker>();
builder.Services.AddHostedService<CocktailExtractionWorker>();

var host = builder.Build();
host.Run();