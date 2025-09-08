using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sellorio.AudioOracle.Data.Content;
using Sellorio.AudioOracle.Data.Metadata;
using Sellorio.AudioOracle.Data.Sessions;
using Sellorio.AudioOracle.Data.TaskQueue;

namespace Sellorio.AudioOracle.Data;

public class DatabaseContext : DbContext
{
    private const string DbPath = "/data/app.db";

    // Content
    public DbSet<FileContentData> FileContents { get; set; }
    public DbSet<FileInfoData> FileInfos { get; set; }
    
    // Metadata
    public DbSet<AlbumArtistData> AlbumArtists { get; set; }
    public DbSet<AlbumData> Albums { get; set; }
    public DbSet<ArtistData> Artists { get; set; }
    public DbSet<ArtistNameData> ArtistNames { get; set; }
    public DbSet<TrackArtistData> TrackArtists { get; set; }
    public DbSet<TrackData> Tracks { get; set; }

    // Sessions
    public DbSet<SessionData> Sessions { get; set; }

    // TaskQueue
    public DbSet<QueuedTaskData> QueuedTasks { get; set; }

    public async Task WithTransaction(Func<DatabaseContextTransaction, Task> action)
    {
        var transaction =
            new DatabaseContextTransaction(
                Database.CurrentTransaction == null
                    ? await Database.BeginTransactionAsync()
                    : null);

        try
        {
            await action.Invoke(transaction);

            if (!transaction.IsComplete)
            {
                await transaction.CommitAsync();
            }
        }
        catch
        {
            if (!transaction.IsComplete)
            {
                await transaction.RollbackAsync();
            }

            throw;
        }
    }

    public async Task<TResult> WithTransaction<TResult>(Func<DatabaseContextTransaction, Task<TResult>> action)
    {
        TResult? result = default;
        await WithTransaction(async (databaseContextTransaction) =>
        {
            result = await action.Invoke(databaseContextTransaction);
        });
        return result!;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }
}
