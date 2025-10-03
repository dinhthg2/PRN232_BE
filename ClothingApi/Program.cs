using ClothingApi.Data;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1) Connection string: ENV trước, fallback appsettings.json
var conn =
    Environment.GetEnvironmentVariable("CONNECTION_STR")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(conn));

// 2) CORS: lấy từ ENV "AllowedOrigins" (hoặc appsettings.json), split + trim
var allowedOriginsRaw =
    Environment.GetEnvironmentVariable("AllowedOrigins")
    ?? builder.Configuration["AllowedOrigins"];

var allowedOrigins = (allowedOriginsRaw ?? string.Empty)
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

// 2b) Đăng ký CORS policy
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AppCors", p =>
    {
        if (allowedOrigins.Length > 0)
        {
            p.WithOrigins(allowedOrigins)
             .AllowAnyHeader()
             .AllowAnyMethod()
             .WithExposedHeaders("Location");
        }
        else
        {
            // Dev fallback: chưa set origin thì mở hết (không dùng cho prod)
            p.SetIsOriginAllowed(_ => true)
             .AllowAnyHeader()
             .AllowAnyMethod()
             .WithExposedHeaders("Location");
        }
    });
});

// 3) MVC + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 4) Proxy headers (Render/Vercel)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// 5) Swagger (để test nhanh)
app.UseSwagger();
app.UseSwaggerUI();

// 6) CORS PHẢI trước MapControllers
app.UseCors("AppCors");

// 7) Endpoints
app.MapControllers();
app.MapGet("/api/health", () => Results.Ok(new { ok = true, time = DateTimeOffset.UtcNow }));

// 8) Auto-migrate + seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);
}

// 9) Run
app.Run();
