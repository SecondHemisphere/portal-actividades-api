using System.ComponentModel.DataAnnotations;

namespace ApiPortalActividades.DTOs
{
    public class OrganizerDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? PhotoUrl { get; set; }

        public string? Department { get; set; }
        public string? Position { get; set; }
        public string? Bio { get; set; }
        public string? Shifts { get; set; }
        public string? WorkDays { get; set; }

        public bool? Active { get; set; }
    }

    public class OrganizerCreateDto
    {
        // USER
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        public string Email { get; set; } = null!;

        [RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = "El teléfono debe contener de 7 a 15 números y puede incluir +")]
        public string? Phone { get; set; }

        public string? PhotoUrl { get; set; }

        // ORGANIZER
        [Required(ErrorMessage = "El departamento es obligatorio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El departamento debe tener entre 2 y 50 caracteres")]
        public string Department { get; set; } = null!;

        [Required(ErrorMessage = "El cargo es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El cargo debe tener entre 3 y 50 caracteres")]
        public string Position { get; set; } = null!;

        [StringLength(300, MinimumLength = 10, ErrorMessage = "La biografía debe tener entre 10 y 300 caracteres")]
        public string? Bio { get; set; }

        public string? Shifts { get; set; }
        public string? WorkDays { get; set; }
    }

    public class OrganizerUpdateDto
    {
        // USER
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        public string Email { get; set; } = null!;

        [RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = "El teléfono debe contener de 7 a 15 números y puede incluir +")]
        public string? Phone { get; set; }

        public string? PhotoUrl { get; set; }

        // ORGANIZER
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El departamento debe tener entre 2 y 50 caracteres")]
        public string? Department { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "El cargo debe tener entre 3 y 50 caracteres")]
        public string? Position { get; set; }

        [StringLength(300, MinimumLength = 10, ErrorMessage = "La biografía debe tener entre 10 y 300 caracteres")]
        public string? Bio { get; set; }

        public string? Shifts { get; set; }
        public string? WorkDays { get; set; }

        public bool? Active { get; set; }
    }

    public class OrganizerSearchDto
    { 
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public string? Shift { get; set; }
    }

    public class OrganizerRegisterDto
    {
        // USER
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; } = null!;

        [RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = "El teléfono debe contener de 7 a 15 números y puede incluir +")]
        public string? Phone { get; set; }

        // ORGANIZER
        [Required(ErrorMessage = "El departamento es obligatorio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El departamento debe tener entre 2 y 50 caracteres")]
        public string Department { get; set; } = null!;

        [Required(ErrorMessage = "El cargo es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El cargo debe tener entre 3 y 50 caracteres")]
        public string Position { get; set; } = null!;

        [Required(ErrorMessage = "La biografía es obligatoria")]
        [StringLength(300, MinimumLength = 10, ErrorMessage = "La biografía debe tener entre 10 y 300 caracteres")]
        public string Bio { get; set; } = null!;
    }

    public class OrganizerProfileUpdateDto
    {
        // USER
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        public string Email { get; set; } = null!;

        [RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = "El teléfono debe contener de 7 a 15 números y puede incluir +")]
        public string? Phone { get; set; }

        public string? PhotoUrl { get; set; }

        // ORGANIZER
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El departamento debe tener entre 2 y 50 caracteres")]
        public string? Department { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "El cargo debe tener entre 3 y 50 caracteres")]
        public string? Position { get; set; }

        [StringLength(300, MinimumLength = 10, ErrorMessage = "La biografía debe tener entre 10 y 300 caracteres")]
        public string? Bio { get; set; }

        [StringLength(100, ErrorMessage = "Los turnos no pueden superar los 100 caracteres")]
        public string? Shifts { get; set; }

        [StringLength(100, ErrorMessage = "Los días de trabajo no pueden superar los 100 caracteres")]
        public string? WorkDays { get; set; }
    }

}
