using ClothingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ClothingApi.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            // Nếu đã có >= 60 sp thì thôi
            if (await db.Products.CountAsync() >= 60) return;

            var now = DateTimeOffset.UtcNow;
            // Tag để phân biệt đợt seed mới => nhìn vào DB thấy ngay là đã đổi
            var dropTag = $"Drop-{now:yyyyMMdd}";

            var batch = new List<Product>
            {
                new() { Name=$"{dropTag} T-Shirt Classic",   Description="Cotton tee",          Price=19.9, Image="https://picsum.photos/seed/dt1/800/500", CreatedAt=now, UpdatedAt=now },
                new() { Name=$"{dropTag} Hoodie Fleece",     Description="Fleece hoodie",       Price=39.5, Image="https://picsum.photos/seed/dh1/800/500", CreatedAt=now, UpdatedAt=now },
                new() { Name=$"{dropTag} Jeans Blue",        Description="Denim blue",          Price=49.0, Image="https://picsum.photos/seed/dj1/800/500", CreatedAt=now, UpdatedAt=now },
                new() { Name=$"{dropTag} Sneakers Lite",     Description="Lightweight shoes",   Price=59.0, Image="https://picsum.photos/seed/ds1/800/500", CreatedAt=now, UpdatedAt=now },
                new() { Name=$"{dropTag} Cap Classic",       Description="Classic cap",         Price=12.0, Image="https://picsum.photos/seed/dc1/800/500", CreatedAt=now, UpdatedAt=now },
                new() { Name=$"{dropTag} Jacket Wind",       Description="Windbreaker",         Price=79.0, Image="https://picsum.photos/seed/djk1/800/500",CreatedAt=now, UpdatedAt=now },
                new() { Name=$"{dropTag} Dress Summer",      Description="Summer dress",        Price=45.0, Image="https://picsum.photos/seed/dd1/800/500", CreatedAt=now, UpdatedAt=now },
                new() { Name=$"{dropTag} Sweater Wool",      Description="Wool sweater",        Price=55.0, Image="https://picsum.photos/seed/dsw1/800/500",CreatedAt=now, UpdatedAt=now },
                new() { Name=$"{dropTag} Blazer Office",     Description="Office blazer",       Price=89.0, Image="https://picsum.photos/seed/dbz1/800/500",CreatedAt=now, UpdatedAt=now },
                new() { Name=$"{dropTag} Coat Long",         Description="Long coat",           Price=120.0,Image="https://picsum.photos/seed/dct1/800/500",CreatedAt=now, UpdatedAt=now },
            };

            // Sinh thêm biến thể (tổng khoảng 40 sp)
            for (int i = 1; i <= 15; i++)
            {
                batch.Add(new Product
                {
                    Name = $"{dropTag} T-Shirt {i}",
                    Description = "Cotton tee",
                    Price = 15 + i,
                    Image = $"https://picsum.photos/seed/dt{i + 1}/800/500",
                    CreatedAt = now.AddMinutes(-i),
                    UpdatedAt = now.AddMinutes(-i)
                });
                batch.Add(new Product
                {
                    Name = $"{dropTag} Sneakers {i}",
                    Description = "Lightweight shoes",
                    Price = 49 + i,
                    Image = $"https://picsum.photos/seed/ds{i + 1}/800/500",
                    CreatedAt = now.AddMinutes(-i * 2),
                    UpdatedAt = now.AddMinutes(-i * 2)
                });
            }

            // Chỉ chèn những tên chưa có (an toàn khi redeploy)
            var existing = await db.Products.Select(p => p.Name).ToListAsync();
            var toInsert = batch.Where(p => !existing.Contains(p.Name)).ToList();

            if (toInsert.Count > 0)
            {
                db.Products.AddRange(toInsert);
                await db.SaveChangesAsync();
            }
        }
    }
}
