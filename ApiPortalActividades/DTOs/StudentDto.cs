using System.ComponentModel.DataAnnotations;

namespace ApiPortalActividades.DTOs
{
    public class StudentDto
    {
        // USER
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? PhotoUrl { get; set; }

        // STUDENT
        public int? FacultyId { get; set; }
        public string? FacultyName { get; set; }

        public int? CareerId { get; set; }
        public string? CareerName { get; set; }

        public int? Semester { get; set; }
        public string? Modality { get; set; }
        public string? Schedule { get; set; }

        public bool? Active { get; set; }
    }

    public class StudentCreateDto
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

        // STUDENT
        [Required(ErrorMessage = "La facultad es obligatoria")]
        public int FacultyId { get; set; }

        [Required(ErrorMessage = "La carrera es obligatoria")]
        public int CareerId { get; set; }

        [Required(ErrorMessage = "El semestre es obligatorio")]
        [Range(1, 10, ErrorMessage = "El semestre debe estar entre 1 y 10")]
        public int Semester { get; set; }

        [Required(ErrorMessage = "La modalidad es obligatoria")]
        [RegularExpression("^(Presencial|Híbrida|Virtual)$", ErrorMessage = "La modalidad debe ser 'Presencial', 'Híbrida' o 'Virtual'")]
        public string Modality { get; set; } = null!;

        [Required(ErrorMessage = "El horario es obligatorio")]
        [RegularExpression("^(Matutina|Vespertina|Nocturna)$", ErrorMessage = "El horario debe ser 'Matutina', 'Vespertina' o 'Nocturna'")]
        public string Schedule { get; set; } = null!;
    }

    public class StudentUpdateDto
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

        // STUDENT
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una facultad válida")]
        public int? FacultyId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una carrera válida")]
        public int? CareerId { get; set; }

        [Range(1, 10, ErrorMessage = "El semestre debe estar entre 1 y 10")]
        public int? Semester { get; set; }

        [RegularExpression("^(Presencial|Híbrida|Virtual)$", ErrorMessage = "La modalidad debe ser 'Presencial', 'Híbrida' o 'Virtual'")]
        public string? Modality { get; set; }

        [RegularExpression("^(Matutina|Vespertina|Nocturna)$", ErrorMessage = "El horario debe ser 'Matutina', 'Vespertina' o 'Nocturna'")]
        public string? Schedule { get; set; }

        public bool? Active { get; set; }
    }

    public class StudentSearchDto
    {
        public string? Name { get; set; }
        public int? FacultyId { get; set; }
        public int? CareerId { get; set; }
        public int? Semester { get; set; }
        public string? Modality { get; set; }
        public string? Schedule { get; set; }
    }

    public class StudentRegisterDto
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

        // STUDENT
        [Required(ErrorMessage = "La facultad es obligatoria")]
        public int FacultyId { get; set; }

        [Required(ErrorMessage = "La carrera es obligatoria")]
        public int CareerId { get; set; }

        [Required(ErrorMessage = "El semestre es obligatorio")]
        [Range(1, 10, ErrorMessage = "El semestre debe estar entre 1 y 10")]
        public int Semester { get; set; }

        [Required(ErrorMessage = "La modalidad es obligatoria")]
        [RegularExpression("^(Presencial|Híbrida|Virtual)$", ErrorMessage = "La modalidad debe ser 'Presencial', 'Híbrida' o 'Virtual'")]
        public string Modality { get; set; } = null!;

        [Required(ErrorMessage = "El horario es obligatorio")]
        [RegularExpression("^(Matutina|Vespertina|Nocturna)$", ErrorMessage = "El horario debe ser 'Matutina', 'Vespertina' o 'Nocturna'")]
        public string Schedule { get; set; } = null!;
    }

    public class StudentProfileUpdateDto
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

        // STUDENT
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una facultad válida")]
        public int? FacultyId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una carrera válida")]
        public int? CareerId { get; set; }

        [Range(1, 10, ErrorMessage = "El semestre debe estar entre 1 y 10")]
        public int? Semester { get; set; }

        [RegularExpression("^(Presencial|Híbrida|Virtual)$", ErrorMessage = "La modalidad debe ser 'Presencial', 'Híbrida' o 'Virtual'")]
        public string? Modality { get; set; }

        [RegularExpression("^(Matutina|Vespertina|Nocturna)$", ErrorMessage = "La jornada debe ser 'Matutina', 'Vespertina' o 'Nocturna'")]
        public string? Schedule { get; set; }
    }

}
