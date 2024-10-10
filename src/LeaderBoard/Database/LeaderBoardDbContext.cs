
using LeaderBoard.Models;

namespace LeaderBoard.Database;

public class LeaderBoardDbContext(DbContextOptions<LeaderBoardDbContext> dbContextOptions) 
    : DbContext(dbContextOptions)
{
    public DbSet<MostSoldProduct> MostSoldProducts { get; set; }
    
    public DbSet<PlayerScore> PlayerScores { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlayerScore>().HasIndex(x => x.Username)
                                          .IsUnique(true);
    }
}
