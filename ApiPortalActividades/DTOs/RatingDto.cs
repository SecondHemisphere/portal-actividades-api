using System.ComponentModel.DataAnnotations;

namespace ApiPortalActividades.DTOs
{
    public class RatingDto
    {
        public int Id { get; set; }

        public int ActivityId { get; set; }
        public string ActivityName { get; set; } = null!;

        public int StudentId { get; set; }
        public string StudentName { get; set; } = null!;

        public int Stars { get; set; }
        public string? Comment { get; set; }
        public DateOnly RatingDate { get; set; }
    }

    public class RatingCreateDto
    {
        [Required(ErrorMessage = "La actividad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una actividad válida")]
        public int ActivityId { get; set; }

        [Required(ErrorMessage = "El estudiante es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un estudiante válido")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "La calificación es obligatoria")]
        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5 estrellas")]
        public int Stars { get; set; }

        [Required(ErrorMessage = "El comentario es obligatorio")]
        [StringLength(300, ErrorMessage = "El comentario no puede superar los 250 caracteres")]
        public string Comment { get; set; } = null!;
    }

    public class RatingSearchDto
    {
        public int? ActivityId { get; set; }
        public int? StudentId { get; set; }
        public int? Stars { get; set; }

        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
    }
}
