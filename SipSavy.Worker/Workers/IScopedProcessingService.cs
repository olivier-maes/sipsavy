namespace SipSavy.Worker.Workers;

internal interface IScopedProcessingService
{
    Task DoWorkAsync(CancellationToken stoppingToken);
}