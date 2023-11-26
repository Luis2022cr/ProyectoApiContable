using System.ComponentModel.DataAnnotations;

namespace ProyectoApiContable.Dtos.Catalogos
{
    public class CatalogoCreateDto
    {
        [Display(Name = "Nombre")]
        [StringLength(70, ErrorMessage = "El {0} requiere {1} caracteres")]
        [Required(ErrorMessage = "El {0} es requerido")]

        public string Name { get; set; }
    }
}
