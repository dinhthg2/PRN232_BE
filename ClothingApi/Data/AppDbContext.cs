using Microsoft.EntityFrameworkCore;
using ClothingApi.Models;

namespace ClothingApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Product> Products => Set<Product>();

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<Product>();
            var now = DateTime.UtcNow;

            foreach (var e in entries)
            {
                if (e.State == EntityState.Modified) e.Entity.UpdatedAt = now;
                if (e.State == EntityState.Added)
                {
                    e.Entity.CreatedAt = now;
                    e.Entity.UpdatedAt = now;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
