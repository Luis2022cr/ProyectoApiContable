using System.ComponentModel.DataAnnotations;

namespace ProyectoApiContable.Dtos.Catalogos;

public class UpdateCuentaDto 
{
    [StringLength(50, ErrorMessage = "El {0} requiere {1} caracteres")]
    [Required(ErrorMessage = "El {0} es requerido")]
    public string Nombre { get; set; }


    [Required(ErrorMessage = "El {0} es requerido")]
    public int Codigo { get; set; }
    public string Descripcion { get; set; }

    [Required(ErrorMessage = "El {0} es requerido")]
    public int TipoCuentaId { get; set; }
}