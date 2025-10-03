using ClothingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ClothingApi.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            await db.Database.MigrateAsync();

            if (!await db.Products.AnyAsync())
            {
                db.Products.AddRange(
                    new Product { Name = "T-Shirt", Description = "Cotton tee", Price = 19.9, Image = "https://picsum.photos/300" },
                    new Product { Name = "Hoodie", Description = "Fleece hoodie", Price = 39.5, Image = "https://picsum.photos/301" },
                    new Product { Name = "Jeans", Description = "Denim blue", Price = 49.0, Image = "https://picsum.photos/302" }
                );
                await db.SaveChangesAsync();
            }
        }
    }
}
