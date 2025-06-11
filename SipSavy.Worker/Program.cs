using SipSavy.ServiceDefaults;
using SipSavy.Worker.AI.Features.Embedding.GetEmbeddings;
using SipSavy.Worker.Data;
using SipSavy.Worker.Features.Video.AddNewVideos;
using SipSavy.Worker.Features.Video.GetVideosByStatus;
using SipSavy.Worker.Features.Video.UpdateVideo;
using SipSavy.Worker.Workers;
using SipSavy.Worker.Youtube.Features.ExtractTranscription;
using SipSavy.Worker.Youtube.Features.GetVideosByChannelId;
using YoutubeExplode;

var builder = Host.CreateApplicationBuilder(args);

// Aspire service defaults
builder.AddServiceDefaults();

// Data
builder.AddNpgsqlDbContext<WorkerDbContext>("sipsavy-worker-db");
builder.Services.AddScoped<IQueryFacade, QueryFacade>();
builder.Services.AddScoped<IVideoRepository, VideoRepository>();

// YoutubeExplode
builder.Services.AddScoped<YoutubeClient>();

// Handlers
builder.Services.AddScoped<GetVideosByChannelIdHandler>();
builder.Services.AddScoped<ExtractTranscriptionHandler>();
builder.Services.AddScoped<AddNewVideosHandler>();
builder.Services.AddScoped<GetVideosByStatusHandler>();
builder.Services.AddScoped<UpdateVideoHandler>();
builder.Services.AddScoped<GetEmbeddingsHandler>();

// Worker
builder.Services.AddHostedService<TranscriptionWorker>();
builder.Services.AddHostedService<EmbeddingWorker>();

var host = builder.Build();
host.Run();