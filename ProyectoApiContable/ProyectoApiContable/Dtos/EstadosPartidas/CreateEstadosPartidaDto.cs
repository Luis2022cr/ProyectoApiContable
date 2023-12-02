using System.ComponentModel.DataAnnotations;

namespace ProyectoApiContable.Dtos.EstadosPartidas;

public class CreateEstadosPartidaDto
{
    
    [Required (ErrorMessage = "El campo {0} es requerido.")]
    public string Nombre { get; set; }
}