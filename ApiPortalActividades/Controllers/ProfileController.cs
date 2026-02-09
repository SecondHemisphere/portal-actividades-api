using ApiPortalActividades.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalActividades.Data.Contexts;

namespace ApiPortalActividades.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly PortalActividadesDbContext _context;

        public ProfileController(PortalActividadesDbContext context)
        {
            _context = context;
        }

        // GET: api/Profile/student
        [Authorize(Roles = "Estudiante")]
        [HttpGet("student")]
        public async Task<ActionResult<StudentDto>> GetMyStudentProfile()
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);

            var student = await _context.Students
                .Include(s => s.User)
                .Include(s => s.Career)
                    .ThenInclude(c => c.Faculty)
                .Where(s => s.UserId == userId)
                .Select(s => new StudentDto
                {
                    Id = s.User.Id,
                    Name = s.User.Name,
                    Email = s.User.Email,
                    Phone = s.User.Phone,
                    PhotoUrl = s.User.PhotoUrl,
                    Active = s.User.Active,
                    CareerId = s.CareerId,
                    CareerName = s.Career != null ? s.Career.Name : null,
                    FacultyId = s.Career != null ? s.Career.FacultyId : null,
                    FacultyName = s.Career != null ? s.Career.Faculty.Name : null,
                    Semester = s.Semester,
                    Modality = s.Modality,
                    Schedule = s.Schedule
                })
                .FirstOrDefaultAsync();

            if (student == null)
                return NotFound(new { message = "Perfil de estudiante no encontrado." });

            return Ok(student);
        }

        // PUT: api/Profile/student
        [Authorize(Roles = "Estudiante")]
        [HttpPut("student")]
        public async Task<IActionResult> UpdateMyStudentProfile([FromBody] StudentProfileUpdateDto dto)
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);

            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
                return NotFound(new { message = "Perfil de estudiante no encontrado." });

            var user = student.User;

            if (dto.Name != user.Name)
            {
                if (await _context.Users.AnyAsync(u => u.Name.ToLower() == dto.Name.ToLower() && u.Id != userId))
                    return BadRequest(new { message = "Ya existe otro usuario con ese nombre." });

                user.Name = dto.Name;
            }

            if (dto.Email != user.Email)
            {
                if (await _context.Users.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower() && u.Id != userId))
                    return BadRequest(new { message = "Ya existe otro usuario con ese correo." });

                user.Email = dto.Email;
            }

            user.Phone = dto.Phone;
            user.PhotoUrl = dto.PhotoUrl;

            if (dto.CareerId.HasValue)
                student.CareerId = dto.CareerId;

            if (dto.Semester.HasValue)
                student.Semester = dto.Semester;

            student.Modality = dto.Modality;
            student.Schedule = dto.Schedule;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Perfil de estudiante actualizado correctamente"
            });
        }

        // GET: api/Profile/organizer
        [Authorize(Roles = "Organizador")]
        [HttpGet("organizer")]
        public async Task<ActionResult<OrganizerDto>> GetMyOrganizerProfile()
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);

            var organizer = await _context.Organizers
                .Include(o => o.User)
                .Where(o => o.UserId == userId)
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
                return NotFound(new { message = "Perfil de organizador no encontrado." });

            return Ok(organizer);
        }

        // PUT: api/Profile/organizer
        [Authorize(Roles = "Organizador")]
        [HttpPut("organizer")]
        public async Task<IActionResult> UpdateMyOrganizerProfile([FromBody] OrganizerProfileUpdateDto dto)
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);

            var organizer = await _context.Organizers
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.UserId == userId);

            if (organizer == null)
                return NotFound(new { message = "Perfil de organizador no encontrado." });

            var user = organizer.User;

            if (dto.Name != user.Name)
            {
                if (await _context.Users.AnyAsync(u => u.Name.ToLower() == dto.Name.ToLower() && u.Id != userId))
                    return BadRequest(new { message = "Ya existe otro usuario con ese nombre." });

                user.Name = dto.Name;
            }

            if (dto.Email != user.Email)
            {
                if (await _context.Users.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower() && u.Id != userId))
                    return BadRequest(new { message = "Ya existe otro usuario con ese correo." });

                user.Email = dto.Email;
            }

            user.Phone = dto.Phone;
            user.PhotoUrl = dto.PhotoUrl;

            organizer.Department = dto.Department;
            organizer.Position = dto.Position;
            organizer.Bio = dto.Bio;
            organizer.Shifts = dto.Shifts;
            organizer.WorkDays = dto.WorkDays;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Perfil de organizador actualizado correctamente"
            });
        }

    }
}
