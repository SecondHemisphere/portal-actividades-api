using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalActividades.Data.Contexts;
using PortalActividades.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPortalActividades.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FacultiesController 
        : ControllerBase
    {
        private readonly PortalActividadesDbContext _context;

        public FacultiesController(PortalActividadesDbContext context)
        {
            _context = context;
        }

        // GET: api/Faculties
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetFacultiesWithCareers()
        {
            var faculties = await _context.Faculties
                .Include(f => f.Careers)
                .ToListAsync();

            var result = faculties.Select(f => new
            {
                id = f.Id,
                name = f.Name,
                careers = f.Careers.Select(c => new {
                    id = c.Id,
                    name = c.Name
                }).ToList()
            });

            return Ok(result);
        }

    }
}
