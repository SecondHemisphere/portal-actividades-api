using System.ComponentModel.DataAnnotations;

namespace ApiPortalActividades.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string Role { get; set; } = null!;
        public bool? Active { get; set; }
        public string? PhotoUrl { get; set; }
    }

    public class UserCreateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        public string Email { get; set; } = null!;

        [Phone(ErrorMessage = "Teléfono inválido")]
        [RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = "El teléfono debe contener de 7 a 15 números y puede incluir +")]
        public string? Phone { get; set; }

        public string? PhotoUrl { get; set; }


        [Required(ErrorMessage = "El rol es obligatorio")]
        [RegularExpression("^(Estudiante|Organizador|Admin)$", ErrorMessage = "El rol debe ser 'Estudiante', 'Organizador' o 'Admin'")]
        public string Role { get; set; } = null!;
    }

    public class UserUpdateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        public string Email { get; set; } = null!;

        [Phone(ErrorMessage = "Teléfono inválido")]
        [RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = "El teléfono debe contener de 7 a 15 números y puede incluir +")]
        public string? Phone { get; set; }
        public string? PhotoUrl { get; set; }
        public bool? Active { get; set; }
    }

    public class UserSearchDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}
