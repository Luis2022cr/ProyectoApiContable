using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoApiContable.Entities;
[Table("tipos_cuentas")]
public class TipoCuenta
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("nombre")]  
    [StringLength(50)]
    [Required]
    public string Nombre { get; set; }
    public ICollection<Cuenta> Cuentas { get; set; }
}