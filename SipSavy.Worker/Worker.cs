using SipSavy.Worker.Youtube.Features.GetVideosByChannelId;

namespace SipSavy.Worker;

internal sealed class Worker(ILogger<Worker> logger, GetVideosByChannelIdHandler handler) : IHostedService, IDisposable
{
    private Timer? _timer = null;

    public Task StartAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);

        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(1));

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        var videos = handler.Handle(new GetVideosByChannelIdRequest("UCioZY1p0bZ4Xt-yodw8_cBQ")).Result;
        Console.WriteLine(videos);
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