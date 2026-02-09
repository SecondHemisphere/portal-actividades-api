using System.ComponentModel.DataAnnotations;

namespace ApiPortalActividades.DTOs
{

    public class EnrollmentDto
    {
        public int Id { get; set; }

        public int ActivityId { get; set; }
        public string ActivityName { get; set; } = null!;

        public DateOnly ActivityDate { get; set; }
        public string ActivityTimeRange { get; set; } = null!;
        public string ActivityLocation { get; set; } = null!;

        public int StudentId { get; set; }
        public string StudentName { get; set; } = null!;

        public DateOnly EnrollmentDate { get; set; }

        public string Status { get; set; } = null!;
        public string? Note { get; set; }
    }

    public class EnrollmentCreateDto
    {
        [Required(ErrorMessage = "La actividad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una actividad válida")]
        public int ActivityId { get; set; }

        [Required(ErrorMessage = "El estudiante es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un estudiante válido")]
        public int StudentId { get; set; }

        [StringLength(300, ErrorMessage = "La nota no puede superar los 300 caracteres")]
        public string? Note { get; set; }
    }

    public class EnrollmentUpdateDto
    {
        [Required(ErrorMessage = "El estado es obligatorio")]
        [RegularExpression("^(Inscrito|Cancelado)$",
            ErrorMessage = "El estado debe ser 'Inscrito' o 'Cancelado'")]
        public string Status { get; set; } = null!;

        [StringLength(300, ErrorMessage = "La nota no puede superar los 300 caracteres")]
        public string? Note { get; set; }
    }

    public class EnrollmentSearchDto
    {
        public int? ActivityId { get; set; }
        public int? StudentId { get; set; }
        public string? Status { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
    }

}
