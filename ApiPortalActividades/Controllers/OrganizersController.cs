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
    public class OrganizersController : ControllerBase
    {
        private readonly PortalActividadesDbContext _context;

        public OrganizersController(PortalActividadesDbContext context)
        {
            _context = context;
        }

        // GET: api/Organizers
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrganizerDto>>> GetOrganizers()
        {
            var organizers = await _context.Organizers
                .Include(o => o.User)
                .OrderByDescending(o => o.User.Active)
                .ThenBy(o => o.User.Id)
                .Select(o => new OrganizerDto
                {
                    Id = o.User.Id,
                    Name = o.User.Name,
                    Email = o.User.Email,
                    Phone = o.User.Phone,
                    PhotoUrl = o.User.PhotoUrl,
                    Active = o.User.Active,
                    Department = o.Department,
                    Position = o.Position,
                    Bio = o.Bio,
                    Shifts = o.Shifts,
                    WorkDays = o.WorkDays
                })
                .ToListAsync();

            return Ok(organizers);
        }

        // GET: api/Organizers/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrganizerDto>> GetOrganizer(int id)
        {
            var organizer = await _context.Organizers
                .Include(o => o.User)
                .Where(o => o.User.Id == id)
                .Select(o => new OrganizerDto
                {
                    Id = o.User.Id,
                    Name = o.User.Name,
                    Email = o.User.Email,
                    Phone = o.User.Phone,
                    PhotoUrl = o.User.PhotoUrl,
                    Active = o.User.Active,
                    Department = o.Department,
                    Position = o.Position,
                    Bio = o.Bio,
                    Shifts = o.Shifts,
                    WorkDays = o.WorkDays
                })
                .FirstOrDefaultAsync();

            if (organizer == null)
                return NotFound(new { message = "Organizador no encontrado." });

            return Ok(organizer);
        }

        // POST: api/Organizers
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> PostOrganizer([FromBody] OrganizerCreateDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower()))
                return BadRequest(new { message = "Ya existe otro usuario con ese correo." });

            if (await _context.Users.AnyAsync(u => u.Name.ToLower() == dto.Name.ToLower()))
                return BadRequest(new { message = "Ya existe otro usuario con ese nombre." });

            var plainPassword = dto.Name.Replace(" ", "").ToLower() + "123";
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = "Organizador",
                PhotoUrl = dto.PhotoUrl,
                Active = true,
                Password = passwordHash
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var organizer = new Organizer
            {
                UserId = user.Id,
                Department = dto.Department,
                Position = dto.Position,
                Bio = dto.Bio,
                Shifts = dto.Shifts,
                WorkDays = dto.WorkDays
            };

            _context.Organizers.Add(organizer);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Organizador creado correctamente",
                defaultPassword = plainPassword
            });
        }

        // PUT: api/Organizers/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrganizer(int id, [FromBody] OrganizerUpdateDto dto)
        {
            var organizer = await _context.Organizers
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.UserId == id);

            if (organizer == null)
                return NotFound(new { message = "Organizador no encontrado." });

            var user = organizer.User;

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                if (await _context.Users.AnyAsync(u => u.Name.ToLower() == dto.Name.ToLower() && u.Id != user.Id))
                    return BadRequest(new { message = "Ya existe otro usuario con ese nombre." });

                user.Name = dto.Name;
            }

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                if (await _context.Users.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower() && u.Id != user.Id))
                    return BadRequest(new { message = "Ya existe otro usuario con ese correo." });

                user.Email = dto.Email;
            }

            user.Phone = dto.Phone ?? user.Phone;
            user.PhotoUrl = dto.PhotoUrl ?? user.PhotoUrl;
            if (dto.Active.HasValue)
                user.Active = dto.Active.Value;

            organizer.Department = dto.Department ?? organizer.Department;
            organizer.Position = dto.Position ?? organizer.Position;
            organizer.Bio = dto.Bio ?? organizer.Bio;
            organizer.Shifts = dto.Shifts ?? organizer.Shifts;
            organizer.WorkDays = dto.WorkDays ?? organizer.WorkDays;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Organizador actualizado correctamente" });
        }

        // POST: api/Organizers/{id}/reset-password
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(int id)
        {
            var organizer = await _context.Organizers
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.UserId == id);

            if (organizer == null)
                return NotFound(new { message = "Organizador no encontrado." });

            var firstName = organizer.User.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries).First().ToLower();
            var newPassword = firstName + "123";

            organizer.User.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Contraseña reseteada correctamente",
                temporaryPassword = newPassword
            });
        }

        // PUT: api/Organizers/deactivate/5
        [Authorize(Roles = "Admin")]
        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivateOrganizer(int id)
        {
            var organizer = await _context.Organizers
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.UserId == id);

            if (organizer == null)
                return NotFound(new { message = "Organizador no encontrado." });

            organizer.User.Active = false;
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Organizador desactivado correctamente" });
        }

        // GET: api/Organizers/search
        [Authorize(Roles = "Admin")]
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<OrganizerDto>>> SearchOrganizers([FromQuery] OrganizerSearchDto filters)
        {
            var query = _context.Organizers
                .Include(o => o.User)
                .Where(o => o.User.Active == true);

            if (!string.IsNullOrWhiteSpace(filters.Name))
                query = query.Where(o => o.User.Name.Contains(filters.Name));

            if (!string.IsNullOrWhiteSpace(filters.Email))
                query = query.Where(o => o.User.Email.Contains(filters.Email));

            if (!string.IsNullOrWhiteSpace(filters.Department))
                query = query.Where(o => o.Department != null && o.Department.Contains(filters.Department));

            if (!string.IsNullOrWhiteSpace(filters.Position))
                query = query.Where(o => o.Position != null && o.Position.Contains(filters.Position));

            if (!string.IsNullOrWhiteSpace(filters.Shift))
                query = query.Where(o => o.Shifts != null && o.Shifts.Contains(filters.Shift));

            var organizers = await query
                .Select(o => new OrganizerDto
                {
                    Id = o.User.Id,
                    Name = o.User.Name,
                    Email = o.User.Email,
                    Phone = o.User.Phone,
                    PhotoUrl = o.User.PhotoUrl,
                    Active = o.User.Active,
                    Department = o.Department,
                    Position = o.Position,
                    Bio = o.Bio,
                    Shifts = o.Shifts,
                    WorkDays = o.WorkDays
                })
                .ToListAsync();

            return Ok(organizers);
        }
    }
}
