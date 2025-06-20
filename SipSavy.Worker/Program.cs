using Microsoft.EntityFrameworkCore;
using SipSavy.Data;
using SipSavy.ServiceDefaults;
using SipSavy.Data.Repository;
using SipSavy.Worker.Features.Chunk.ChunkTextByFixedSize;
using SipSavy.Worker.Features.Chunk.GetContextChunks;
using SipSavy.Worker.Features.Cocktail.AddNewCocktails;
using SipSavy.Worker.Features.Cocktail.ExtractCocktails;
using SipSavy.Worker.Features.Embedding.GetEmbeddings;
using SipSavy.Worker.Features.Video.AddNewVideos;
using SipSavy.Worker.Features.Video.GetVideosByStatus;
using SipSavy.Worker.Features.Video.UpdateVideo;
using SipSavy.Worker.Features.VideoChunk.AddVideoChunks;
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
builder.AddNpgsqlDbContext<AppDbContext>("sipsavy",
    configureDbContextOptions: dbContextOptionsBuilder =>
    {
        dbContextOptionsBuilder.UseNpgsql(op => { op.UseVector(); });
    });

builder.Services.AddScoped<IQueryFacade, QueryFacade>();
builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddScoped<ICocktailRepository, CocktailRepository>();
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
builder.Services.AddScoped<ExtractCocktailsHandler>();
builder.Services.AddScoped<GetContextChunksHandler>();
builder.Services.AddScoped<AddNewCocktailsHandler>();

// Worker
builder.Services.AddHostedService<TranscriptionWorker>();
builder.Services.AddHostedService<EmbeddingWorker>();
builder.Services.AddHostedService<CocktailExtractionWorker>();

// Ensure database is created and pgvector extension is installed
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

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