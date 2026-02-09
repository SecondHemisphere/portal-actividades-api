using ApiPortalActividades.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalActividades.Data.Contexts;
using PortalActividades.Data.Models;

namespace ApiPortalActividades.Controllers
{
    [Authorize(Roles = "Admin,Organizador")]
    [Route("api/[controller]")]
    [ApiController]
    public class ActivitiesController : ControllerBase
    {
        private readonly PortalActividadesDbContext _context;

        public ActivitiesController(PortalActividadesDbContext context)
        {
            _context = context;
        }

        // GET: api/Activities
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Activity>>> GetActivities()
        {
            return await _context.Activities.ToListAsync();
        }

        // GET: api/Activities/Activities2
        [AllowAnonymous]
        [HttpGet("Activities2")]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetActivities2()
        {
            var activities = await _context.Activities
                .Include(a => a.Category)
                .Include(a => a.Organizer)
                .OrderByDescending(a => a.Date)
                .Select(a => new ActivityDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    CategoryId = a.CategoryId,
                    CategoryName = a.Category.Name,
                    OrganizerId = a.OrganizerId,
                    OrganizerName = a.Organizer.Name,
                    Date = a.Date,
                    RegistrationDeadline = a.RegistrationDeadline,
                    TimeRange = $"{a.StartTime:HH\\:mm} - {a.EndTime:HH\\:mm}",
                    Location = a.Location,
                    Capacity = a.Capacity,
                    Description = a.Description,
                    PhotoUrl = a.PhotoUrl,
                    Active = a.Active ?? false
                })
                .ToListAsync();

            return Ok(activities);
        }

        // GET: api/Activities/active
        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetActiveActivities()
        {
            var activities = await _context.Activities
                .Include(a => a.Category)
                .Include(a => a.Organizer)
                .Where(a => a.Active == true)
                .OrderBy(a => a.Date)
                .Select(a => new ActivityDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    CategoryId = a.CategoryId,
                    CategoryName = a.Category.Name,
                    OrganizerId = a.OrganizerId,
                    OrganizerName = a.Organizer.Name,
                    Date = a.Date,
                    RegistrationDeadline = a.RegistrationDeadline,
                    TimeRange = $"{a.StartTime:HH\\:mm} - {a.EndTime:HH\\:mm}",
                    Location = a.Location,
                    Capacity = a.Capacity,
                    Description = a.Description,
                    PhotoUrl = a.PhotoUrl,
                    Active = a.Active ?? true
                })
                .ToListAsync();

            return Ok(activities);
        }

        // GET: api/Activities/available
        [AllowAnonymous]
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetAvailableActivities()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var activities = await _context.Activities
                .Include(a => a.Category)
                .Include(a => a.Organizer)
                .Where(a => a.Active == true && a.RegistrationDeadline >= today)
                .OrderBy(a => a.Date)
                .Select(a => new ActivityDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    CategoryId = a.CategoryId,
                    CategoryName = a.Category.Name,
                    OrganizerId = a.OrganizerId,
                    OrganizerName = a.Organizer.Name,
                    Date = a.Date,
                    RegistrationDeadline = a.RegistrationDeadline,
                    TimeRange = $"{a.StartTime:HH\\:mm} - {a.EndTime:HH\\:mm}",
                    Location = a.Location,
                    Capacity = a.Capacity,
                    Description = a.Description,
                    PhotoUrl = a.PhotoUrl,
                    Active = a.Active ?? false
                })
                .ToListAsync();

            return Ok(activities);
        }

        // GET: api/Activities/5
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityDto>> GetActivity(int id)
        {
            var activity = await _context.Activities
                .Include(a => a.Category)
                .Include(a => a.Organizer)
                .Where(a => a.Id == id)
                .Select(a => new ActivityDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    CategoryId = a.CategoryId,
                    CategoryName = a.Category.Name,
                    OrganizerId = a.OrganizerId,
                    OrganizerName = a.Organizer.Name,
                    Date = a.Date,
                    RegistrationDeadline = a.RegistrationDeadline,
                    TimeRange = $"{a.StartTime:HH\\:mm} - {a.EndTime:HH\\:mm}",
                    Location = a.Location,
                    Capacity = a.Capacity,
                    Description = a.Description,
                    PhotoUrl = a.PhotoUrl,
                    Active = a.Active ?? false
                })
                .FirstOrDefaultAsync();

            if (activity == null)
                return NotFound(new { message = "Actividad no encontrada." });

            return Ok(activity);
        }

        // GET: api/Activities/organizer/{organizerId}/month/{year}/{month}
        [Authorize(Roles = "Admin,Organizador")]
        [HttpGet("organizer/{organizerId}/month/{year}/{month}")]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetActivitiesByOrganizerAndMonth(
            int organizerId,
            int year,
            int month)
        {
            if (year < 2000 || year > 2100)
                return BadRequest(new { message = "Año fuera de rango válido (2000-2100)." });

            if (month < 1 || month > 12)
                return BadRequest(new { message = "Mes debe estar entre 1 y 12." });

            try
            {
                var startDate = new DateOnly(year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var activities = await _context.Activities
                    .Include(a => a.Category)
                    .Include(a => a.Organizer)
                    .Where(a => a.OrganizerId == organizerId &&
                               a.Date >= startDate &&
                               a.Date <= endDate)
                    .OrderBy(a => a.Date)
                    .ThenBy(a => a.StartTime)
                    .Select(a => new ActivityDto
                    {
                        Id = a.Id,
                        Title = a.Title,
                        CategoryId = a.CategoryId,
                        CategoryName = a.Category.Name,
                        OrganizerId = a.OrganizerId,
                        OrganizerName = a.Organizer.Name,
                        Date = a.Date,
                        RegistrationDeadline = a.RegistrationDeadline,
                        TimeRange = $"{a.StartTime:HH\\:mm} - {a.EndTime:HH\\:mm}",
                        Location = a.Location,
                        Capacity = a.Capacity,
                        Description = a.Description,
                        PhotoUrl = a.PhotoUrl,
                        Active = a.Active ?? false
                    })
                    .ToListAsync();

                return Ok(activities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor.", details = ex.Message });
            }
        }

        // POST: api/Activities
        [Authorize(Roles = "Admin,Organizador")]
        [HttpPost]
        public async Task<IActionResult> PostActivity([FromBody] ActivityCreateDto dto)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);

            if (!categoryExists)
            {
                return BadRequest(new
                {
                    message = "La categoría seleccionada no existe."
                });
            }

            var organizerExists = await _context.Organizers.AnyAsync(o => o.UserId == dto.OrganizerId);

            if (!organizerExists)
            {
                return BadRequest(new
                {
                    message = "El organizador seleccionado no existe."
                });
            }

            var times = dto.TimeRange.Split('-');
            if (times.Length != 2)
            {
                return BadRequest(new { message = "El formato de TimeRange es incorrecto. Debe ser 'HH:mm - HH:mm'." });
            }

            if (!TimeOnly.TryParseExact(times[0].Trim(), "HH:mm", null, System.Globalization.DateTimeStyles.None, out var startTime) ||
                !TimeOnly.TryParseExact(times[1].Trim(), "HH:mm", null, System.Globalization.DateTimeStyles.None, out var endTime))
            {
                return BadRequest(new { message = "Formato de hora inválido. Use formato 24 horas 'HH:mm'." });
            }

            var response = new Activity
            {
                Title = dto.Title,
                CategoryId = dto.CategoryId,
                OrganizerId = dto.OrganizerId,
                Date = dto.Date,
                RegistrationDeadline = dto.RegistrationDeadline,
                StartTime = startTime,
                EndTime = endTime,
                Location = dto.Location,
                Capacity = dto.Capacity,
                Description = dto.Description,
                PhotoUrl = dto.PhotoUrl,
                Active = dto.Active
            };

            _context.Activities.Add(response);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Actividad creada correctamente",
                data = response
            });
        }

        // PUT: api/Activities/5
        [Authorize(Roles = "Admin,Organizador")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActivity(int id, [FromBody] ActivityUpdateDto dto)
        {
            var activity = await _context.Activities
                .Include(a => a.Category)
                .Include(a => a.Organizer)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null)
                return NotFound(new { message = "Actividad no encontrada." });

            if (dto.CategoryId.HasValue)
            {
                var categoryExists = await _context.Categories
                    .AnyAsync(c => c.Id == dto.CategoryId.Value);

                if (!categoryExists)
                {
                    return BadRequest(new
                    {
                        message = "La categoría seleccionada no existe."
                    });
                }

                activity.CategoryId = dto.CategoryId.Value;
            }

            if (dto.OrganizerId.HasValue)
            {
                var organizerExists = await _context.Organizers
                    .AnyAsync(o => o.UserId == dto.OrganizerId.Value);

                if (!organizerExists)
                {
                    return BadRequest(new
                    {
                        message = "El organizador seleccionado no existe."
                    });
                }

                activity.OrganizerId = dto.OrganizerId.Value;
            }


            if (!string.IsNullOrWhiteSpace(dto.Title))
                activity.Title = dto.Title;
            if (dto.Date.HasValue)
                activity.Date = dto.Date.Value;
            if (dto.RegistrationDeadline.HasValue)
                activity.RegistrationDeadline = dto.RegistrationDeadline.Value;
            if (!string.IsNullOrWhiteSpace(dto.PhotoUrl))
                activity.PhotoUrl = dto.PhotoUrl;

            if (!string.IsNullOrWhiteSpace(dto.TimeRange))
            {
                var times = dto.TimeRange.Split('-');
                if (times.Length == 2 &&
                    TimeOnly.TryParseExact(times[0].Trim(), "HH:mm", null, System.Globalization.DateTimeStyles.None, out var startTime) &&
                    TimeOnly.TryParseExact(times[1].Trim(), "HH:mm", null, System.Globalization.DateTimeStyles.None, out var endTime))
                {
                    activity.StartTime = startTime;
                    activity.EndTime = endTime;
                }
            }

            if (dto.Location != null)
                activity.Location = dto.Location;

            if (dto.Capacity.HasValue)
                activity.Capacity = dto.Capacity.Value;
            if (!string.IsNullOrWhiteSpace(dto.Description))
                activity.Description = dto.Description;
            if (dto.Active.HasValue)
                activity.Active = dto.Active.Value;

            await _context.SaveChangesAsync();

            await _context.Entry(activity)
                .Reference(a => a.Category)
                .LoadAsync();

            await _context.Entry(activity)
                .Reference(a => a.Organizer)
                .LoadAsync();

            var response = new ActivityDto
            {
                Id = activity.Id,
                Title = activity.Title,
                CategoryId = activity.CategoryId,
                CategoryName = activity.Category.Name,
                OrganizerId = activity.OrganizerId,
                OrganizerName = activity.Organizer.Name,
                Date = activity.Date,
                RegistrationDeadline = activity.RegistrationDeadline,
                TimeRange = $"{activity.StartTime:HH\\:mm} - {activity.EndTime:HH\\:mm}",
                Location = activity.Location,
                Capacity = activity.Capacity,
                Description = activity.Description,
                PhotoUrl = activity.PhotoUrl,
                Active = activity.Active ?? false
            };

            return Ok(new
            {
                success = true,
                message = "Actividad actualizada correctamente",
                data = response
            });
        }

        // PUT: api/Activities/deactivate/5
        [Authorize(Roles = "Admin,Organizador")]
        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivateActivity(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
                return NotFound(new { message = "Actividad no encontrada." });

            activity.Active = false;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Actividad eliminada correctamente"
            });
        }

        // PUT: api/Activities/activate/5
        [Authorize(Roles = "Admin,Organizador")]
        [HttpPut("activate/{id}")]
        public async Task<IActionResult> ActivateActivity(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
                return NotFound(new { message = "Actividad no encontrada." });

            activity.Active = true;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Actividad activada correctamente",
            });
        }

        // GET: api/Activities/search
        [Authorize(Roles = "Admin")]
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> SearchActivities([FromQuery] ActivitySearchDto filters)
        {
            var query = _context.Activities
                .Include(a => a.Category)
                .Include(a => a.Organizer)
                .AsQueryable();

            if (filters.FromDate.HasValue && filters.ToDate.HasValue && filters.ToDate < filters.FromDate)
            {
                return BadRequest(new { message = "La fecha final no puede ser menor que la fecha inicial." });
            }

            if (filters.CategoryId.HasValue)
                query = query.Where(a => a.CategoryId == filters.CategoryId);
            if (filters.OrganizerId.HasValue)
                query = query.Where(a => a.OrganizerId == filters.OrganizerId);
            if (filters.FromDate.HasValue)
                query = query.Where(a => a.Date >= filters.FromDate.Value);
            if (filters.ToDate.HasValue)
                query = query.Where(a => a.Date <= filters.ToDate.Value);
            if (!string.IsNullOrWhiteSpace(filters.Title))
                query = query.Where(a => a.Title.Contains(filters.Title));
            if (!string.IsNullOrWhiteSpace(filters.Location))
                query = query.Where(a => a.Location.Contains(filters.Location));

            var activities = await query
                .Select(a => new ActivityDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    CategoryId = a.CategoryId,
                    CategoryName = a.Category.Name,
                    OrganizerId = a.OrganizerId,
                    OrganizerName = a.Organizer.Name,
                    Date = a.Date,
                    RegistrationDeadline = a.RegistrationDeadline,
                    TimeRange = $"{a.StartTime:HH\\:mm} - {a.EndTime:HH\\:mm}",
                    Location = a.Location,
                    Capacity = a.Capacity,
                    Description = a.Description,
                    PhotoUrl = a.PhotoUrl,
                    Active = a.Active ?? false
                })
                .ToListAsync();

            return Ok(activities);
        }

        // GET: api/Activities/search
        [AllowAnonymous]
        [HttpGet("search/public")]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> SearchPublicActivities(
            [FromQuery] string? title = null,
            [FromQuery] int? categoryId = null)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var query = _context.Activities
                .Include(a => a.Category)
                .Include(a => a.Organizer)
                .Where(a => a.Active == true && a.RegistrationDeadline >= today)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
                query = query.Where(a => a.Title.Contains(title));

            if (categoryId.HasValue)
                query = query.Where(a => a.CategoryId == categoryId.Value);

            var activities = await query
                .OrderBy(a => a.Date)
                .Select(a => new ActivityDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    CategoryId = a.CategoryId,
                    CategoryName = a.Category.Name,
                    OrganizerId = a.OrganizerId,
                    OrganizerName = a.Organizer.Name,
                    Date = a.Date,
                    RegistrationDeadline = a.RegistrationDeadline,
                    TimeRange = $"{a.StartTime:HH\\:mm} - {a.EndTime:HH\\:mm}",
                    Location = a.Location,
                    Capacity = a.Capacity,
                    Description = a.Description,
                    PhotoUrl = a.PhotoUrl,
                    Active = a.Active ?? false
                })
                .ToListAsync();

            return Ok(activities);
        }

    }
}

