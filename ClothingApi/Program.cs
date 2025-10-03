using ClothingApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1) Lấy connection string (ưu tiên ENV để deploy, fallback appsettings.json)
var conn = Environment.GetEnvironmentVariable("CONNECTION_STR")
           ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(conn));

// 2) CORS (cho phép FE gọi API, cấu hình từ appsettings.json: "AllowedOrigins")
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AppCors", p =>
        p.WithOrigins(builder.Configuration["AllowedOrigins"]!.Split(','))
         .AllowAnyHeader()
         .AllowAnyMethod());
});

// 3) Add Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 4) Swagger luôn bật cho dev
app.UseSwagger();
app.UseSwaggerUI();

// 5) Middleware CORS
app.UseCors("AppCors");

// 6) Map Controllers
app.MapControllers();

// 7) Health check endpoint
app.MapGet("/api/health", () => Results.Ok(new { ok = true }));

// 8) Seed dữ liệu mẫu khi start app
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(db);
}

// 9) Run
app.Run();
