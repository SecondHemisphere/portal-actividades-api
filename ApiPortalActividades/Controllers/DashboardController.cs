using ApiPortalActividades.DTOs.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalActividades.Data.Contexts;

[Authorize]
[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly PortalActividadesDbContext _context;

    public DashboardController(PortalActividadesDbContext context)
    {
        _context = context;
    }

    // TOTALS
    [Authorize(Roles = "Admin")]
    [HttpGet("totals")]
    public async Task<ActionResult<DashboardTotalsDto>> GetTotals()
    {
        return new DashboardTotalsDto
        {
            TotalActivities = await _context.Activities.CountAsync(),
            TotalCategories = await _context.Categories.CountAsync(),
            TotalEnrollments = await _context.Enrollments.CountAsync(),
            TotalStudents = await _context.Students.CountAsync(),
            TotalOrganizers = await _context.Organizers.CountAsync(),
            TotalUsers = await _context.Users.CountAsync(),
            TotalRatings = await _context.Ratings.CountAsync()
        };
    }

    // ACTIVITIES BY CATEGORY
    [Authorize(Roles = "Admin")]
    [HttpGet("activities-by-category")]
    public async Task<ActionResult<IEnumerable<ActivitiesByCategoryDto>>> GetActivitiesByCategory()
    {
        return await _context.Activities
            .GroupBy(a => a.Category.Name)
            .Select(g => new ActivitiesByCategoryDto
            {
                CategoryName = g.Key,
                TotalActivities = g.Count()
            })
            .ToListAsync();
    }

    // TOP RATINGS
    [Authorize(Roles = "Admin")]
    [HttpGet("top-ratings")]
    public async Task<ActionResult<IEnumerable<TopRatingsDto>>> GetTopRatings()
    {
        return await _context.Ratings
            .GroupBy(r => r.Activity.Title)
            .Select(g => new TopRatingsDto
            {
                ActivityTitle = g.Key,
                AvgRating = g.Average(x => x.Stars)
            })
            .OrderByDescending(x => x.AvgRating)
            .Take(5)
            .ToListAsync();
    }

}
