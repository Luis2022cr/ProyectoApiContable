using System;
using System.ComponentModel.DataAnnotations;

namespace ProyectoApiContable.Dtos.Catalogos
{
    public class CatalogoCreateDto
    {
        [Display(Name = "Nombre")]
        [StringLength(70, ErrorMessage = "El {0} requiere {1} caracteres")]
        [Required(ErrorMessage = "El {0} es requerido")]

        public string Name { get; set; }

        [Display(Name = "Usuario")]
        [StringLength(70, ErrorMessage = "El {0} requiere {1} caracteres")]
        [Required(ErrorMessage = "El {0} es requerido")]

        public string User { get; set; }

        [Display(Name = "Descripcion")]
        [StringLength(70, ErrorMessage = "El {0} requiere {1} caracteres")]
        [Required(ErrorMessage = "El {0} es requerido")]

        public string Description { get; set; }

        [Display(Name = "Fecha")]
        [StringLength(70, ErrorMessage = "El {0} requiere {1} caracteres")]
        [Required(ErrorMessage = "El {0} es requerido")]

        public DateTime Fecha { get; set; }
    }
}
