using Microsoft.EntityFrameworkCore;
using Sellorio.AudioOracle.Data.Sessions;

namespace Sellorio.AudioOracle.Data;

public class DatabaseContext : DbContext
{
    private const string DbPath = "/data/app.db";

    public DbSet<SessionData> Sessions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }
}
