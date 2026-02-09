using ApiPortalActividades.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalActividades.Data.Contexts;
using PortalActividades.Data.Models;

namespace ApiPortalActividades.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly PortalActividadesDbContext _context;

        public CategoriesController(PortalActividadesDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _context.Categories
                .OrderByDescending(c => c.Active)
                .ThenBy(c => c.Id)
                .ToListAsync();
        }

        // GET: api/Categories/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // POST: api/Categories
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> PostCategory([FromBody] CategoryCreateDto dto)
        {
            bool exists = await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower());

            if (exists)
            {
                return BadRequest(new
                {
                    errors = new
                    {
                        name = new[] { "Ya existe otra categoría con ese nombre." }
                    }
                });
            }

            var category = new Category
            {
                Name = dto.Name,
                Active = dto.Active
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Categoría creada correctamente",
                data = category
            });
        }

        // PUT: api/Categories/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, [FromBody] CategoryUpdateDto dto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound(new { message = "Categoría no encontrada." });

            bool exists = await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower() && c.Id != id);

            if (exists)
            {
                return BadRequest(new
                {
                    errors = new
                    {
                        name = new[] { "Ya existe otra categoría con ese nombre." }
                    }
                });
            }

            category.Name = dto.Name;
            category.Active = dto.Active;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Categoría actualizada correctamente",
                data = category
            });
        }

        // PUT: api/Categories/deactivate/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivateCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound("Categoría no encontrada.");

            category.Active = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error al desactivar la categoría.");
            }

            return Ok(new
            {
                success = true,
                message = "Categoría eliminada correctamente"
            });
        }

        // GET: api/Categories/search
        [Authorize(Roles = "Admin")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchCategories(string? name)
        {
            var query = _context.Categories
                .Where(c => c.Active == true)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(c => c.Name.Contains(name));

            var categories = await query
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Active
                })
                .ToListAsync();

            if (!categories.Any())
                return NotFound("No se encontraron categorías con los criterios dados.");

            return Ok(categories);
        }

    }
}
