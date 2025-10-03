using Microsoft.AspNetCore.Mvc;
using ClothingApi.Data;
using ClothingApi.Models;
using ClothingApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ClothingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProductsController(AppDbContext context) => _context = context;

        // GET /api/Products?search=&page=1&limit=20&sort=createdAt_desc
        [HttpGet]
        public async Task<ActionResult<object>> GetProducts(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20,
            [FromQuery] string? sort = "createdAt_desc")
        {
            limit = Math.Clamp(limit, 1, 100);
            var q = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(p => p.Name.ToLower().Contains(s) || p.Description.ToLower().Contains(s));
            }

            // sort: name_asc|name_desc|price_asc|price_desc|createdAt_asc|createdAt_desc
            q = sort switch
            {
                "name_asc" => q.OrderBy(p => p.Name),
                "name_desc" => q.OrderByDescending(p => p.Name),
                "price_asc" => q.OrderBy(p => p.Price),
                "price_desc" => q.OrderByDescending(p => p.Price),
                "createdAt_asc" => q.OrderBy(p => p.CreatedAt),
                _ => q.OrderByDescending(p => p.CreatedAt)
            };

            var total = await q.CountAsync();
            var items = await q.Skip((page - 1) * limit).Take(limit).ToListAsync();
            return Ok(new { items, page, limit, total });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var p = await _context.Products.FindAsync(id);
            return p is null ? NotFound() : p;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Create(ProductCreateDto dto)
        {
            var p = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Image = dto.Image
            };
            _context.Products.Add(p);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = p.Id }, p);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
        {
            var p = await _context.Products.FindAsync(id);
            if (p is null) return NotFound();

            if (dto.Name != null) p.Name = dto.Name;
            if (dto.Description != null) p.Description = dto.Description;
            if (dto.Price.HasValue) p.Price = dto.Price.Value;
            if (dto.Image is not null) p.Image = dto.Image;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _context.Products.FindAsync(id);
            if (p is null) return NotFound();
            _context.Products.Remove(p);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
