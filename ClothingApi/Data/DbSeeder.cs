using Microsoft.EntityFrameworkCore;
using ClothingApi.Models;

namespace ClothingApi.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            await db.Database.MigrateAsync();

            // Chỉ seed khi số lượng ít (tránh trùng lặp nhiều lần)
            if (await db.Products.CountAsync() >= 24) return;

            var items = new List<Product>
            {
                new() { Name="T-Shirt", Description="Cotton tee", Price=19.9, Image="https://picsum.photos/seed/t1/600/400" },
                new() { Name="Hoodie", Description="Fleece hoodie", Price=39.5, Image="https://picsum.photos/seed/h1/600/400" },
                new() { Name="Jeans", Description="Denim blue", Price=49.0, Image="https://picsum.photos/seed/j1/600/400" },
                new() { Name="Sneakers", Description="Lightweight shoes", Price=59.0, Image="https://picsum.photos/seed/s1/600/400" },
                new() { Name="Cap", Description="Classic cap", Price=12.0, Image="https://picsum.photos/seed/c1/600/400" },
                new() { Name="Jacket", Description="Windbreaker", Price=79.0, Image="https://picsum.photos/seed/jk1/600/400" },
                new() { Name="Skirt", Description="Pleated", Price=34.0, Image="https://picsum.photos/seed/sk1/600/400" },
                new() { Name="Dress", Description="Summer dress", Price=45.0, Image="https://picsum.photos/seed/d1/600/400" },
                new() { Name="Shorts", Description="Linen shorts", Price=22.0, Image="https://picsum.photos/seed/sh1/600/400" },
                new() { Name="Sweater", Description="Wool sweater", Price=55.0, Image="https://picsum.photos/seed/sw1/600/400" },
                new() { Name="Blazer", Description="Office wear", Price=89.0, Image="https://picsum.photos/seed/bz1/600/400" },
                new() { Name="Coat", Description="Long coat", Price=120.0, Image="https://picsum.photos/seed/ct1/600/400" },
            };

            // Thêm biến thể để đủ ~24 mẫu
            for (int i = 2; i <= 12; i++)
            {
                items.Add(new Product
                {
                    Name = $"T-Shirt {i}",
                    Description = "Cotton tee",
                    Price = 15 + i,
                    Image = $"https://picsum.photos/seed/t{i}/600/400"
                });
            }

            db.Products.AddRange(items);
            await db.SaveChangesAsync();
        }
    }
}
