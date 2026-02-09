using System.ComponentModel.DataAnnotations;

namespace ApiPortalActividades.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool Active { get; set; }
    }

    public class CategoryCreateDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(50, MinimumLength = 3,
            ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")]
        [RegularExpression(
          @"^[a-zA-Z0-9 áéíóúÁÉÍÓÚñÑ]+$",
          ErrorMessage = "El nombre solo puede contener letras, números y espacios."
        )]
        public string Name { get; set; } = null!;

        public bool Active { get; set; } = true;
    }

    public class CategoryUpdateDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(50, MinimumLength = 3,
            ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")]
        [RegularExpression(
          @"^[a-zA-Z0-9 áéíóúÁÉÍÓÚñÑ]+$",
          ErrorMessage = "El nombre solo puede contener letras, números y espacios."
        )]
        public string Name { get; set; } = null!;

        public bool Active { get; set; }
    }

}

