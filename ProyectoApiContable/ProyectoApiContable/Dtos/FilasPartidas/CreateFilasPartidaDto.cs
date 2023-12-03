using System.ComponentModel.DataAnnotations;

namespace ProyectoApiContable.Dtos.FilasPartidas;

public class CreateFilasPartidaDto
{
    [Required(ErrorMessage = "El campo 'Debito' es obligatorio.")]
    public decimal Debito { get; set; }

    [Required(ErrorMessage = "El campo 'Credito' es obligatorio.")]
    public decimal Credito { get; set; }

    [Required(ErrorMessage = "El campo 'CuentaId' es obligatorio.")]
    public Guid CuentaId { get; set; }


}