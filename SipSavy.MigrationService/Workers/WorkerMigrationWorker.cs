using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SipSavy.Worker.Data;

namespace SipSavy.MigrationService.Workers;

internal sealed class WorkerMigrationWorker(IServiceProvider serviceProvider,
     IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
     private const string ActivitySourceName = "SipSavy Worker Migrations";
     private static readonly ActivitySource SActivitySource = new(ActivitySourceName);
     
     protected override async Task ExecuteAsync(CancellationToken cancellationToken)
     {
          using var activity = SActivitySource.StartActivity("Migrating sipsavy worker database", ActivityKind.Client);

          try
          {
               using var scope = serviceProvider.CreateScope();
               var dbContext = scope.ServiceProvider.GetRequiredService<WorkerDbContext>();

               await RunMigrationAsync(dbContext, cancellationToken);
               //await SeedDataAsync(dbContext, cancellationToken);
          }
          catch (Exception ex)
          {
               activity?.AddException(ex);
               throw;
          }

          hostApplicationLifetime.StopApplication();
     }
     
     private static async Task RunMigrationAsync(WorkerDbContext dbContext, CancellationToken cancellationToken)
     {
          var strategy = dbContext.Database.CreateExecutionStrategy();
          await strategy.ExecuteAsync(async () =>
          {
               // Run migration in a transaction to avoid partial migration if it fails.
               await dbContext.Database.MigrateAsync(cancellationToken);
          });
     }

     private static async Task SeedDataAsync(WorkerDbContext dbContext, CancellationToken cancellationToken)
     {
          var strategy = dbContext.Database.CreateExecutionStrategy();
          await strategy.ExecuteAsync(async () =>
          {
               // Seed the database
               await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            
               // Add data to seed here
               await dbContext.SaveChangesAsync(cancellationToken);
               await transaction.CommitAsync(cancellationToken);
          });
     }
}