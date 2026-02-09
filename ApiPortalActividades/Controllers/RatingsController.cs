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
    public class RatingsController : ControllerBase
    {
        private readonly PortalActividadesDbContext _context;

        public RatingsController(PortalActividadesDbContext context)
        {
            _context = context;
        }

        // GET: api/Ratings
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rating>>> GetRatings()
        {
            return await _context.Ratings.ToListAsync();
        }

        // GET: api/Ratings/Ratings2
        [Authorize(Roles = "Admin")]
        [HttpGet("Ratings2")]
        public async Task<ActionResult<IEnumerable<RatingDto>>> GetRatings2()
        {
            var ratings = await _context.Ratings
                .Include(r => r.Activity)
                .Include(r => r.Student)
                .OrderByDescending(r => r.RatingDate)
                .Select(r => new RatingDto
                {
                    Id = r.Id,
                    ActivityId = r.ActivityId,
                    ActivityName = r.Activity.Title,
                    StudentId = r.StudentId,
                    StudentName = r.Student.Name,
                    Stars = r.Stars,
                    Comment = r.Comment,
                    RatingDate = r.RatingDate
                })
                .ToListAsync();

            return Ok(ratings);
        }

        // GET: api/Ratings/5
        [Authorize(Roles = "Admin,Estudiante")]
        [HttpGet("{id}")]
        public async Task<ActionResult<RatingDto>> GetRating(int id)
        {
            var rating = await _context.Ratings
                .Include(r => r.Activity)
                .Include(r => r.Student)
                .Where(r => r.Id == id)
                .Select(r => new RatingDto
                {
                    Id = r.Id,
                    ActivityId = r.ActivityId,
                    ActivityName = r.Activity.Title,
                    StudentId = r.StudentId,
                    StudentName = r.Student.Name,
                    Stars = r.Stars,
                    Comment = r.Comment,
                    RatingDate = r.RatingDate
                })
                .FirstOrDefaultAsync();

            if (rating == null)
                return NotFound("Valoración no encontrada.");

            return Ok(rating);
        }

        // GET: api/Ratings/student/5/ratings
        [Authorize(Roles = "Admin,Estudiante")]
        [HttpGet("student/{studentId}/ratings")]
        public async Task<ActionResult<IEnumerable<RatingDto>>> GetRatingsByStudent(int studentId)
        {
            var ratings = await _context.Ratings
                .Include(r => r.Activity)
                .Include(r => r.Student)
                .Where(r => r.StudentId == studentId)
                .Select(r => new RatingDto
                {
                    Id = r.Id,
                    ActivityId = r.ActivityId,
                    ActivityName = r.Activity.Title,
                    StudentId = r.StudentId,
                    StudentName = r.Student.Name,
                    Stars = r.Stars,
                    Comment = r.Comment,
                    RatingDate = r.RatingDate
                })
                .ToListAsync();

            return Ok(ratings);
        }

        // GET: api/Ratings/activity/5/ratings
        [AllowAnonymous]
        [HttpGet("activity/{activityId}/ratings")]
        public async Task<ActionResult<IEnumerable<RatingDto>>> GetRatingsByActivity(int activityId)
        {
            var ratings = await _context.Ratings
                .Include(r => r.Activity)
                .Include(r => r.Student)
                .Where(r => r.ActivityId == activityId)
                .Select(r => new RatingDto
                {
                    Id = r.Id,
                    ActivityId = r.ActivityId,
                    ActivityName = r.Activity.Title,
                    StudentId = r.StudentId,
                    StudentName = r.Student.Name,
                    Stars = r.Stars,
                    Comment = r.Comment,
                    RatingDate = r.RatingDate
                })
                .ToListAsync();

            return Ok(ratings);
        }

        // POST: api/Ratings
        [Authorize(Roles = "Estudiante")]
        [HttpPost]
        public async Task<IActionResult> PostRating([FromBody] RatingCreateDto dto)
        {
            bool exists = await _context.Ratings
                .AnyAsync(r => r.ActivityId == dto.ActivityId && r.StudentId == dto.StudentId);

           if (exists)
               return BadRequest(new { message = "Usted ya calificó esta actividad." });

            var rating = new Rating
            {
                ActivityId = dto.ActivityId,
                StudentId = dto.StudentId,
                Stars = dto.Stars,
                Comment = dto.Comment,
                RatingDate = DateOnly.FromDateTime(DateTime.Now)
            };

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Valoración registrada correctamente" });
        }

        // DELETE: api/Ratings/5
        [Authorize(Roles = "Admin,Estudiante")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
            {
                return NotFound(new { message = "Valoración no encontrada" });
            }

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Valoración eliminada correctamente"
            });
        }

        // GET: api/Ratings/search
        [Authorize(Roles = "Admin")]
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<RatingDto>>> SearchRatings([FromQuery] RatingSearchDto filters)
        {
            var query = _context.Ratings
                .Include(r => r.Activity)
                .Include(r => r.Student)
                .AsQueryable();

            if (filters.FromDate.HasValue && filters.ToDate.HasValue)
            {
                if (filters.ToDate < filters.FromDate)
                {
                    return BadRequest(new
                    {
                        message = "La fecha final no puede ser menor que la fecha inicial."
                    });
                }
            }

            if (filters.ActivityId.HasValue)
                query = query.Where(r => r.ActivityId == filters.ActivityId);

            if (filters.StudentId.HasValue)
                query = query.Where(r => r.StudentId == filters.StudentId);

            if (filters.Stars.HasValue)
                query = query.Where(r => r.Stars == filters.Stars);

            if (filters.FromDate.HasValue)
                query = query.Where(r => r.RatingDate >= filters.FromDate);

            if (filters.ToDate.HasValue)
                query = query.Where(r => r.RatingDate <= filters.ToDate);

            var ratings = await query
                .Select(r => new RatingDto
                {
                    Id = r.Id,
                    ActivityId = r.ActivityId,
                    ActivityName = r.Activity.Title,
                    StudentId = r.StudentId,
                    StudentName = r.Student.Name,
                    Stars = r.Stars,
                    Comment = r.Comment,
                    RatingDate = r.RatingDate
                })
                .ToListAsync();

            return Ok(ratings);
        }

    }
}
