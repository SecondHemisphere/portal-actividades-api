using ApiPortalActividades.DTOs;
using ApiPortalActividades.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PortalActividades.Data.Contexts;
using PortalActividades.Data.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiPortalActividades.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly PortalActividadesDbContext _context;

        public AuthController(IConfiguration configuration, PortalActividadesDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            var usuario = _context.Users.FirstOrDefault(u => u.Email == login.Email);

            if (usuario == null)
                return Unauthorized(new { message = "El usuario no existe" });

            if (!BCrypt.Net.BCrypt.Verify(login.Password, usuario.Password))
                return Unauthorized(new { message = "Credenciales inválidas" });

            if (usuario.Active == false)
                return Unauthorized(new { message = "Usuario inactivo. Contacte al administrador." });

            var token = GenerateToken(usuario);
            return Ok(new { token });
        }

        [AllowAnonymous]
        [HttpPost("register/student")]
        public async Task<IActionResult> RegisterStudent([FromBody] StudentRegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest(new { message = "El correo ya está registrado" });

            if (await _context.Users.AnyAsync(u => u.Name == dto.Name))
                return BadRequest(new { message = "Ya existe otro usuario con ese nombre" });

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = "Estudiante",
                Active = true,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            var student = new Student
            {
                User = user,
                CareerId = dto.CareerId,
                Semester = dto.Semester,
                Modality = dto.Modality,
                Schedule = dto.Schedule
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registro de estudiante exitoso" });
        }

        [AllowAnonymous]
        [HttpPost("register/organizer")]
        public async Task<IActionResult> RegisterOrganizer([FromBody] OrganizerRegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest(new { message = "El correo ya está registrado" });

            if (await _context.Users.AnyAsync(u => u.Name == dto.Name))
                return BadRequest(new { message = "Ya existe otro usuario con ese nombre" });

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = "Organizador",
                Active = true,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            var organizer = new Organizer
            {
                User = user,
                Department = dto.Department,
                Position = dto.Position,
                Bio = dto.Bio
            };

            _context.Organizers.Add(organizer);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registro de organizador exitoso" });
        }

        private string GenerateToken(User usuario)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings").
                Get<JwtSettings>();
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("id", usuario.Id.ToString()),
                new Claim("username", usuario.Name),
                new Claim("role", usuario.Role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpireMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

}