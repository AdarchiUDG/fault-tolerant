using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DockerExample; 

public sealed class WikiContext : DbContext {
  public DbSet<Entry> Entries => Set<Entry>();
  
  public WikiContext(DbContextOptions<WikiContext> dbContextOptions) : base(dbContextOptions) {
    ChangeTracker.StateChanged += OnStateChanged;
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    modelBuilder.Entity<Entry>().ToTable("Entries");
  }

  private static void OnStateChanged(object? sender, EntityStateChangedEventArgs args) {
    if (args is { NewState: EntityState.Modified, Entry.Entity: BaseEntity entity }) {
      entity.UpdatedAt = DateTime.Now;
    }
  }
}
