using Microsoft.EntityFrameworkCore;
using SipSavy.ServiceDefaults;
using SipSavy.Worker.AI.Features.Chunk.ChunkTextByFixedSize;
using SipSavy.Worker.AI.Features.Embedding.GetEmbeddings;
using SipSavy.Worker.Data;
using SipSavy.Worker.Data.Repository;
using SipSavy.Worker.Features.Video.AddNewVideos;
using SipSavy.Worker.Features.Video.GetVideosByStatus;
using SipSavy.Worker.Features.Video.UpdateVideo;
using SipSavy.Worker.Features.VideoChunk.AddVideoChunks;
using SipSavy.Worker.Workers;
using SipSavy.Worker.Youtube.Features.ExtractTranscription;
using SipSavy.Worker.Youtube.Features.GetVideosByChannelId;
using YoutubeExplode;

var builder = Host.CreateApplicationBuilder(args);

// Aspire service defaults
builder.AddServiceDefaults();

// Ollama
builder.AddOllamaApiClient("embedding").AddEmbeddingGenerator();

// Data
builder.AddNpgsqlDbContext<WorkerDbContext>("sipsavy-worker-db",
    configureDbContextOptions: dbContextOptionsBuilder =>
    {
        dbContextOptionsBuilder.UseNpgsql(op => { op.UseVector(); });
    });

builder.Services.AddScoped<IQueryFacade, QueryFacade>();
builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddScoped<IVectorStore, PostgresVectorStore>();

// YoutubeExplode
builder.Services.AddScoped<YoutubeClient>();

// Handlers
builder.Services.AddScoped<GetVideosByChannelIdHandler>();
builder.Services.AddScoped<ExtractTranscriptionHandler>();
builder.Services.AddScoped<AddNewVideosHandler>();
builder.Services.AddScoped<GetVideosByStatusHandler>();
builder.Services.AddScoped<UpdateVideoHandler>();
builder.Services.AddScoped<GetEmbeddingsHandler>();
builder.Services.AddScoped<ChunkTextByFixedSizeHandler>();
builder.Services.AddScoped<AddVideoChunksHandler>();

// Worker
builder.Services.AddHostedService<TranscriptionWorker>();
builder.Services.AddHostedService<EmbeddingWorker>();

// Ensure database is created and pgvector extension is installed
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WorkerDbContext>();

    if (dbContext.Database.GetPendingMigrations().Any())
    {
        await dbContext.Database.MigrateAsync();
        
        await dbContext.Database.OpenConnectionAsync();
        await ((Npgsql.NpgsqlConnection)dbContext.Database.GetDbConnection()).ReloadTypesAsync();
        await dbContext.Database.CloseConnectionAsync();
    }
}

var host = builder.Build();
host.Run();