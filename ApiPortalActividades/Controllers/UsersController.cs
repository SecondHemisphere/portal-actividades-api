using ApiPortalActividades.DTOs;
using Azure;
using Humanizer;
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
    public class UsersController : ControllerBase
    {
        private readonly PortalActividadesDbContext _context;

        public UsersController(PortalActividadesDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users
                .OrderByDescending(u => u.Active)
                .ThenBy(u => u.Id)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Phone = u.Phone,
                    Role = u.Role,
                    Active = u.Active,
                    PhotoUrl = u.PhotoUrl
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/Users/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Phone = u.Phone,
                    Role = u.Role,
                    Active = u.Active,
                    PhotoUrl = u.PhotoUrl
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound("Usuario no encontrado.");

            return Ok(user);
        }

        // POST: api/Users
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] UserCreateDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower()))
                return BadRequest(new { message = "Ya existe un usuario con ese correo." });

            if (await _context.Users.AnyAsync(u => u.Name.ToLower() == dto.Name.ToLower()))
                return BadRequest(new { message = "Ya existe un usuario con ese nombre." });

            var plainPassword = dto.Name.Replace(" ", "").ToLower() + "123";
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = dto.Role,
                PhotoUrl = dto.PhotoUrl,
                Active = true,
                Password = passwordHash
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Usuario creado correctamente",
                defaultPassword = plainPassword
            });
        }

        // PUT: api/Users/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, [FromBody] UserUpdateDto dto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("Usuario no encontrado.");

            if (dto.Name != user.Name)
            {
                bool nameExists = await _context.Users
                    .AnyAsync(u => u.Name.ToLower() == dto.Name.ToLower() && u.Id != id);

                if (nameExists)
                {
                    return BadRequest(new { message = "Ya existe otro usuario con ese nombre." });
                }

                user.Name = dto.Name;
            }

            if (dto.Email != user.Email)
            {
                bool emailExists = await _context.Users
                    .AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower() && u.Id != id);

                if (emailExists)
                {
                    return BadRequest(new { message = "Ya existe otro usuario con ese correo." });
                }

                user.Email = dto.Email;
            }

            user.Phone = dto.Phone;
            user.PhotoUrl = dto.PhotoUrl;

            if (dto.Active.HasValue)
                user.Active = dto.Active.Value;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Usuario actualizado correctamente",
            });
        }

        // POST: api/Users/{id}/reset-password
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("Usuario no encontrado.");

            var firstName = user.Name
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .First()
                .ToLower();

            var newPassword = firstName + "123";
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Contraseña reseteada correctamente",
                temporaryPassword = newPassword
            });
        }

        // PUT: api/Users/deactivate/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("Usuario no encontrado.");

            user.Active = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error al desactivar el usuario.");
            }

            return Ok(new
            {
                success = true,
                message = "Usuario eliminado correctamente"
            });
        }

        // GET: api/Users/search
        [Authorize(Roles = "Admin")]
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<UserDto>>> SearchUsers([FromQuery] UserSearchDto filters)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.Name))
                query = query.Where(u => u.Name.Contains(filters.Name));

            if (!string.IsNullOrWhiteSpace(filters.Email))
                query = query.Where(u => u.Email.Contains(filters.Email));

            if (!string.IsNullOrWhiteSpace(filters.Role))
                query = query.Where(u => u.Role == filters.Role);

            var users = await query
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Phone = u.Phone,
                    Role = u.Role,
                    Active = u.Active,
                    PhotoUrl = u.PhotoUrl
                })
                .ToListAsync();

            return Ok(users);
        }
    }
}
