using System.ComponentModel.DataAnnotations;

namespace ProyectoApiContable.Dtos.Usuarios
{
    public class RecuperarDto
    {
        [Display(Name = "Correo Electronico")]
        [Required(ErrorMessage = "El {0} es requerido")]
        public string Email { get; set; }
    }
}
