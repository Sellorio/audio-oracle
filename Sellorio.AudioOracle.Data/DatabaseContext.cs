using Microsoft.EntityFrameworkCore;
using Sellorio.AudioOracle.Data.Metadata;
using Sellorio.AudioOracle.Data.Sessions;

namespace Sellorio.AudioOracle.Data;

public class DatabaseContext : DbContext
{
    private const string DbPath = "/data/app.db";
    
    // Metadata
    public DbSet<AlbumArtistData> AlbumArtists { get; set; }
    public DbSet<AlbumData> Albums { get; set; }
    public DbSet<ArtistData> Artists { get; set; }
    public DbSet<ArtistNameData> ArtistNames { get; set; }
    public DbSet<TrackArtistData> TrackArtists { get; set; }
    public DbSet<TrackData> Tracks { get; set; }

    // Sessions
    public DbSet<SessionData> Sessions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }
}
