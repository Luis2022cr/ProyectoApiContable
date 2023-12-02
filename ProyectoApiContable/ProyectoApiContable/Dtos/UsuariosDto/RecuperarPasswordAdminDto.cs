using System.ComponentModel.DataAnnotations;

namespace ProyectoApiContable.Dtos.UsuariosDto
{
    public class RecuperarPasswordAdminDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
