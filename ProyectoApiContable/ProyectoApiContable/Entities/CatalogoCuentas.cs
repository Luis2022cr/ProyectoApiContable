using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProyectoApiContable.Entities
{
    [Table("catalogo_cuentas", Schema = "contable")]
    public class CatalogoCuentas
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        [StringLength(50)]
        [Required]
        public string Nombre { get; set; }

        [Column("usuario")]
        [StringLength(50)]
        [Required]
        public string Usuario { get; set; }

        [Column("total")]
        [Required]
        public decimal Total { get; set; }

        [Column("descripcion")]
        [StringLength(255)]
        public string Descripcion { get; set; }

        [Column("fecha")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime Fecha { get; set; }
    }
}