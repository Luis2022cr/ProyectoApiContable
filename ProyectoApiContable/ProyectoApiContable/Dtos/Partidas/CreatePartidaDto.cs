using System.ComponentModel.DataAnnotations;
using ProyectoApiContable.Dtos.FilasPartidas;

namespace ProyectoApiContable.Dtos.Partidas;

public class CreatePartidaDto
{
        [Required(ErrorMessage = "El campo 'Nombre' es obligatorio.")]
        [StringLength(50, ErrorMessage = "El campo 'Nombre' no puede tener más de 50 caracteres.")]
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        
        public List<CreateFilasPartidaDto> FilasPartida { get; set; }
        
}