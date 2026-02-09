using System.ComponentModel.DataAnnotations;

namespace ApiPortalActividades.DTOs
{
    public class ActivityDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public int OrganizerId { get; set; }
        public string OrganizerName { get; set; } = null!;
        public DateOnly Date { get; set; }
        public DateOnly RegistrationDeadline { get; set; }
        public string TimeRange { get; set; } = null!;
        public string Location { get; set; } = null!;
        public int Capacity { get; set; }
        public string? Description { get; set; }
        public string? PhotoUrl { get; set; }
        public bool Active { get; set; }
    }

    public class ActivityCreateDto
    {
        [Required(ErrorMessage = "El título es requerido")]
        [StringLength(80, MinimumLength = 3, ErrorMessage = "El título debe tener entre 3 y 80 caracteres")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "La categoría es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría válida")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "El organizador es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un organizador válido")]
        public int OrganizerId { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        public DateOnly Date { get; set; }

        [Required(ErrorMessage = "La fecha límite de inscripción es requerida")]
        public DateOnly RegistrationDeadline { get; set; }

        [Required(ErrorMessage = "El rango de horario es requerido")]
        [RegularExpression(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]\s*-\s*([0-1]?[0-9]|2[0-3]):[0-5][0-9]$",
            ErrorMessage = "Formato de horario inválido. Use 'HH:mm - HH:mm'")]
        public string TimeRange { get; set; } = null!;

        [Required(ErrorMessage = "El lugar es requerido")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "El lugar debe tener al menos 3 caracteres")]
        public string Location { get; set; } = null!;

        [Required(ErrorMessage = "La capacidad es requerida")]
        [Range(10, 500, ErrorMessage = "La capacidad debe estar entre 10 y 500")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "La descripción debe tener al menos 10 caracteres")]
        public string? Description { get; set; }

        public string? PhotoUrl { get; set; }

        public bool Active { get; set; } = true;
    }

    public class ActivityUpdateDto
    {
        [StringLength(80, MinimumLength = 3, ErrorMessage = "El título debe tener entre 3 y 80 caracteres")]
        public string? Title { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría válida")]
        public int? CategoryId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un organizador válido")]
        public int? OrganizerId { get; set; }

        public DateOnly? Date { get; set; }

        public DateOnly? RegistrationDeadline { get; set; }

        [RegularExpression(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]\s*-\s*([0-1]?[0-9]|2[0-3]):[0-5][0-9]$",
            ErrorMessage = "Formato de horario inválido. Use 'HH:mm - HH:mm'")]
        public string? TimeRange { get; set; }

        [StringLength(200, MinimumLength = 3, ErrorMessage = "El lugar debe tener al menos 3 caracteres")]
        public string Location { get; set; } = null!;

        [Range(10, 500, ErrorMessage = "La capacidad debe estar entre 10 y 500")]
        public int? Capacity { get; set; }

        [StringLength(2000, MinimumLength = 10, ErrorMessage = "La descripción debe tener al menos 10 caracteres")]
        public string? Description { get; set; }

        public string? PhotoUrl { get; set; }

        public bool? Active { get; set; }
    }

    public class ActivitySearchDto
    {
        public int? CategoryId { get; set; }

        public int? OrganizerId { get; set; }

        public DateOnly? FromDate { get; set; }

        public DateOnly? ToDate { get; set; }

        public string? Location { get; set; }

        public string? Title { get; set; }
    }

    public class PublicActivitySearchDto
    {
        public string? Title { get; set; }

        public int? CategoryId { get; set; }
    }

}