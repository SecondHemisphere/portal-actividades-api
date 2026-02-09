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
    public class StudentsController : ControllerBase
    {
        private readonly PortalActividadesDbContext _context;

        public StudentsController(PortalActividadesDbContext context)
        {
            _context = context;
        }

        // GET: api/Students
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents()
        {
            var students = await _context.Students
                .Include(s => s.User)
                .Include(s => s.Career)
                    .ThenInclude(c => c.Faculty)
                .OrderByDescending(s => s.User.Active)
                .ThenBy(s => s.User.Id)
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
                .ToListAsync();

            return Ok(students);
        }

        // GET: api/Students/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetStudent(int id)
        {
            var student = await _context.Students
                .Include(s => s.User)
                .Include(s => s.Career)
                    .ThenInclude(c => c.Faculty)
                .Where(s => s.User.Id == id)
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
                return NotFound(new { message = "Estudiante no encontrado." });

            return Ok(student);
        }

        // POST: api/Students
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> PostStudent([FromBody] StudentCreateDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower()))
                return BadRequest(new { message = "Ya existe otro usuario con ese correo." });

            if (await _context.Users.AnyAsync(u => u.Name.ToLower() == dto.Name.ToLower()))
                return BadRequest(new { message = "Ya existe otro usuario con ese nombre." });

            var career = await _context.Careers
                .Include(c => c.Faculty)
                .FirstOrDefaultAsync(c => c.Id == dto.CareerId);

            if (career == null)
                return BadRequest(new { message = "La carrera no existe." });

            var plainPassword = dto.Name.Replace(" ", "").ToLower() + "123";

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = "Estudiante",
                PhotoUrl = dto.PhotoUrl,
                Active = true,
                Password = BCrypt.Net.BCrypt.HashPassword(plainPassword)
            };

            var student = new Student
            {
                User = user,
                CareerId = career.Id,
                Semester = dto.Semester,
                Modality = dto.Modality,
                Schedule = dto.Schedule
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Estudiante creado correctamente",
                defaultPassword = plainPassword
            });
        }

        // PUT: api/Students/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, [FromBody] StudentUpdateDto dto)
        {
            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == id);

            if (student == null)
                return NotFound(new { message = "Estudiante no encontrado." });

            var user = student.User;

            if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != user.Name)
            {
                if (await _context.Users.AnyAsync(u => u.Name.ToLower() == dto.Name.ToLower() && u.Id != user.Id))
                    return BadRequest(new { message = "Ya existe otro usuario con ese nombre." });

                user.Name = dto.Name;
            }

            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
            {
                if (await _context.Users.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower() && u.Id != user.Id))
                    return BadRequest(new { message = "Ya existe otro usuario con ese correo." });

                user.Email = dto.Email;
            }

            user.Phone = dto.Phone ?? user.Phone;
            user.PhotoUrl = dto.PhotoUrl ?? user.PhotoUrl;
            if (dto.Active.HasValue)
                user.Active = dto.Active.Value;

            if (dto.CareerId.HasValue)
                student.CareerId = dto.CareerId;

            if (dto.Semester.HasValue)
                student.Semester = dto.Semester;

            student.Modality = dto.Modality ?? student.Modality;
            student.Schedule = dto.Schedule ?? student.Schedule;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Estudiante actualizado correctamente" });
        }

        // POST: api/Students/{id}/reset-password
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(int id)
        {
            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == id);

            if (student == null)
                return NotFound(new { message = "Estudiante no encontrado." });

            var firstName = student.User.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries).First().ToLower();
            var newPassword = firstName + "123";

            student.User.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Contraseña reseteada correctamente",
                temporaryPassword = newPassword
            });
        }

        // PUT: api/Students/deactivate/5
        [Authorize(Roles = "Admin")]
        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivateStudent(int id)
        {
            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == id);

            if (student == null)
                return NotFound(new { message = "Estudiante no encontrado." });

            student.User.Active = false;
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Estudiante desactivado correctamente" });
        }

        // GET: api/Students/search
        [Authorize(Roles = "Admin")]
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<StudentDto>>> SearchStudents([FromQuery] StudentSearchDto filters)
        {
            var query = _context.Students
                .Include(s => s.User)
                .Include(s => s.Career)
                    .ThenInclude(c => c.Faculty)
                .Where(s => s.User.Active == true);

            if (!string.IsNullOrWhiteSpace(filters.Name))
                query = query.Where(s => s.User.Name.Contains(filters.Name));

            if (filters.FacultyId.HasValue)
                query = query.Where(s => s.Career != null && s.Career.FacultyId == filters.FacultyId);

            if (filters.CareerId.HasValue)
                query = query.Where(s => s.CareerId == filters.CareerId);

            if (filters.Semester.HasValue)
                query = query.Where(s => s.Semester == filters.Semester);

            if (!string.IsNullOrWhiteSpace(filters.Modality))
                query = query.Where(s => s.Modality != null && s.Modality.Contains(filters.Modality));

            if (!string.IsNullOrWhiteSpace(filters.Schedule))
                query = query.Where(s => s.Schedule != null && s.Schedule == filters.Schedule);

            var students = await query
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
                .ToListAsync();

            return Ok(students);
        }
    }
}
