using SipSavy.Worker.Youtube.Features.ExtractTranscription;
using SipSavy.Worker.Youtube.Features.GetVideosByChannelId;

namespace SipSavy.Worker;

internal sealed class Worker(
    ILogger<Worker> logger,
    GetVideosByChannelIdHandler getVideosByChannelIdHandler,
    ExtractTranscriptionHandler extractTranscriptionHandler)
    : IHostedService, IDisposable
{
    private Timer? _timer;

    public Task StartAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);

        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(1));

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        //var videos = handler.Handle(new GetVideosByChannelIdRequest("UCioZY1p0bZ4Xt-yodw8_cBQ")).Result;
        var response = extractTranscriptionHandler.Handle(new ExtractTranscriptionRequest("W_v1EwDUNLo")).Result;
        Console.WriteLine(response);
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Worker stopping");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}