using ApiPortalActividades.DTOs;
using Azure;
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
    public class EnrollmentsController : ControllerBase
    {
        private readonly PortalActividadesDbContext _context;

        public EnrollmentsController(PortalActividadesDbContext context)
        {
            _context = context;
        }

        // GET: api/Enrollments
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Enrollment>>> GetEnrollments()
        {
            return await _context.Enrollments.ToListAsync();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Enrollments2")]
        public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetEnrollments2()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Activity)
                .Include(e => e.Student)
                .OrderByDescending(e => e.EnrollmentDate)
                .Select(e => new EnrollmentDto
                {
                    Id = e.Id,
                    ActivityId = e.ActivityId,
                    ActivityName = e.Activity.Title,
                    StudentId = e.StudentId,
                    StudentName = e.Student.Name,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status,
                    Note = e.Note
                })
                .ToListAsync();

            return Ok(enrollments);
        }

        // GET: api/Enrollments/5
        [Authorize(Roles = "Admin,Estudiante")]
        [HttpGet("{id}")]
        public async Task<ActionResult<EnrollmentDto>> GetEnrollment(int id)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Activity)
                .Include(e => e.Student)
                .Where(e => e.Id == id)
                .Select(e => new EnrollmentDto
                {
                    Id = e.Id,
                    ActivityId = e.ActivityId,
                    ActivityName = e.Activity.Title,
                    StudentId = e.StudentId,
                    StudentName = e.Student.Name,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status,
                    Note = e.Note

                })
                .FirstOrDefaultAsync();

            if (enrollment == null)
                return NotFound(new { message = "Inscripción no encontrada." });

            return Ok(enrollment);
        }

        // GET: api/Enrollments/student/5/enrollments
        [Authorize(Roles = "Admin,Estudiante")]
        [HttpGet("student/{studentId}/enrollments")]
        public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetEnrollmentsByStudent(int studentId)
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Activity)
                .Include(e => e.Student)
                .Where(e => e.StudentId == studentId)
                .Select(e => new EnrollmentDto
                {
                    Id = e.Id,
                    ActivityId = e.ActivityId,
                    ActivityName = e.Activity.Title,
                    ActivityDate = e.Activity.Date,
                    ActivityTimeRange = $"{e.Activity.StartTime:HH\\:mm} - {e.Activity.EndTime:HH\\:mm}",
                    ActivityLocation = e.Activity.Location,
                    StudentId = e.StudentId,
                    StudentName = e.Student.Name,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status,
                    Note = e.Note
                })
                .ToListAsync();

            return Ok(enrollments);
        }

        // POST: api/Enrollments
        [Authorize(Roles = "Admin,Estudiante")]
        [HttpPost]
        public async Task<IActionResult> PostEnrollment([FromBody] EnrollmentCreateDto dto)
        {
            var activity = await _context.Activities
                .FirstOrDefaultAsync(a => a.Id == dto.ActivityId);

            if (activity == null)
                return BadRequest(new { message = "Actividad no encontrada" });

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == dto.StudentId);

            if (student == null)
                return BadRequest(new { message = "Estudiante no encontrado" });

            var today = DateOnly.FromDateTime(DateTime.Now);

            if (today > activity.RegistrationDeadline)
                return BadRequest(new
                {
                    message = $"No se permiten más inscripciones en la actividad {activity.Title} porque tiene como fecha límite el día {activity.RegistrationDeadline:dd/MM/yyyy}."
                });

            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.ActivityId == dto.ActivityId && e.StudentId == dto.StudentId);

            if (existingEnrollment != null)
            {
                if (existingEnrollment.Status == "Cancelado")
                {
                    existingEnrollment.Status = "Inscrito";
                    existingEnrollment.Note = dto.Note;
                    existingEnrollment.EnrollmentDate = today;

                    await _context.SaveChangesAsync();

                    var updatedEnrollmentDto = await _context.Enrollments
                        .Include(e => e.Activity)
                        .Include(e => e.Student)
                        .Where(e => e.Id == existingEnrollment.Id)
                        .Select(e => new EnrollmentDto
                        {
                            Id = e.Id,
                            ActivityId = e.ActivityId,
                            ActivityName = e.Activity.Title,
                            ActivityDate = e.Activity.Date,
                            ActivityTimeRange = $"{e.Activity.StartTime:HH\\:mm} - {e.Activity.EndTime:HH\\:mm}",
                            ActivityLocation = e.Activity.Location,
                            StudentId = e.StudentId,
                            StudentName = e.Student.Name,
                            EnrollmentDate = e.EnrollmentDate,
                            Status = e.Status,
                            Note = e.Note
                        })
                        .FirstOrDefaultAsync();

                    return Ok(new
                    {
                        success = true,
                        message = "Inscripción reactivada correctamente",
                        enrollment = updatedEnrollmentDto
                    });
                }
                else
                {
                    return BadRequest(new { message = "El estudiante ya está inscrito en esta actividad." });
                }
            }

            var enrollment = new Enrollment
            {
                ActivityId = dto.ActivityId,
                StudentId = dto.StudentId,
                EnrollmentDate = today,
                Status = "Inscrito",
                Note = dto.Note
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            var newEnrollmentDto = await _context.Enrollments
                .Include(e => e.Activity)
                .Include(e => e.Student)
                .Where(e => e.Id == enrollment.Id)
                .Select(e => new EnrollmentDto
                {
                    Id = e.Id,
                    ActivityId = e.ActivityId,
                    ActivityName = e.Activity.Title,
                    ActivityDate = e.Activity.Date,
                    ActivityTimeRange = $"{e.Activity.StartTime:HH\\:mm} - {e.Activity.EndTime:HH\\:mm}",
                    ActivityLocation = e.Activity.Location,
                    StudentId = e.StudentId,
                    StudentName = e.Student.Name,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status,
                    Note = e.Note
                })
                .FirstOrDefaultAsync();

            return Ok(new
            {
                success = true,
                message = "Inscripción creada correctamente",
                data = newEnrollmentDto
            });
        }

        // PUT: api/Enrollments/5
        [Authorize(Roles = "Admin,Estudiante")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEnrollment(int id, [FromBody] EnrollmentUpdateDto dto)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Activity)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enrollment == null)
                return NotFound(new { message = "Inscripción no encontrada." });

            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                if (enrollment.Activity != null && today > enrollment.Activity.Date)
                {
                    return BadRequest(new
                    {
                        message = $"No se puede cambiar el estado de la inscripción porque la actividad '{enrollment.Activity.Title}' ya finalizó el día {enrollment.Activity.Date:dd/MM/yyyy}."
                    });
                }

                enrollment.Status = dto.Status;
            }

            if (!string.IsNullOrWhiteSpace(dto.Note))
                enrollment.Note = dto.Note;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Inscripción actualizada correctamente",
                data = enrollment
            });
        }

        // DELETE: api/Enrollments/deactivate/5
        [Authorize(Roles = "Admin, Estudiante")]
        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivateEnrollment(int id)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Activity)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enrollment == null)
                return NotFound(new { message = "Inscripción no encontrada." });

            var today = DateOnly.FromDateTime(DateTime.Now);
            if (enrollment.Activity != null && today > enrollment.Activity.Date)
            {
                return BadRequest(new
                {
                    message = $"No se puede cancelar la inscripción porque la actividad '{enrollment.Activity.Title}' ya finalizó el día {enrollment.Activity.Date:dd/MM/yyyy}."
                });
            }

            enrollment.Status = "Cancelado";
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Inscripción cancelada correctamente"
            });
        }

        // GET: api/Enrollments/search
        [Authorize(Roles = "Admin")]
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<EnrollmentDto>>> SearchEnrollments([FromQuery] EnrollmentSearchDto filters)
        {
            var query = _context.Enrollments
                .Include(e => e.Activity)
                .Include(e => e.Student)
                .AsQueryable();

            if (filters.FromDate.HasValue && filters.ToDate.HasValue && filters.ToDate < filters.FromDate)
            {
                return BadRequest(new { message = "La fecha final no puede ser menor que la fecha inicial." });
            }

            if (filters.ActivityId.HasValue)
                query = query.Where(e => e.ActivityId == filters.ActivityId);

            if (filters.StudentId.HasValue)
                query = query.Where(e => e.StudentId == filters.StudentId);

            if (!string.IsNullOrWhiteSpace(filters.Status))
                query = query.Where(e => e.Status == filters.Status);

            if (filters.FromDate.HasValue)
                query = query.Where(e => e.EnrollmentDate >= filters.FromDate);

            if (filters.ToDate.HasValue)
                query = query.Where(e => e.EnrollmentDate <= filters.ToDate);

            var enrollments = await query
                .Select(e => new EnrollmentDto
                {
                    Id = e.Id,
                    ActivityId = e.ActivityId,
                    ActivityName = e.Activity.Title,
                    StudentId = e.StudentId,
                    StudentName = e.Student.Name,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status,
                    Note = e.Note
                })
                .ToListAsync();

            return Ok(enrollments);
        }

    }
}