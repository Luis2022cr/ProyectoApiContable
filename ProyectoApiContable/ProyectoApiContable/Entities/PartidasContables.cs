using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoApiContable.Entities
{
    [Table("partidas_contables")]
    public class PartidasContables
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre_partida")]
        [StringLength(50)]
        [Required]
        public string NombrePartida { get; set; }

        [Column("codigo")]
        [Required]
        public int Codigo { get; set; }

        [Column("fecha_creacion")]
        [DataType(DataType.Date)]
        public DateTime FechaCreacion { get; set; }

        [Column("Cuenta_debito")]
        [ForeignKey("CuentaDebito")]
        public int CuentaDebitoId { get; set; }
        public CatalogoCuentas CuentaDebito { get; set; }

        [Column("Cuenta_credito")]
        [ForeignKey("CuentaCredito")]
        public int CuentaCreditoId { get; set; }
        public CatalogoCuentas CuentaCredito { get; set; }
        
        [Column("monto")]
        [Required]
        public decimal Monto { get; set; }
        

        [Column("descripcion")]
        [StringLength(255)]
        public string Descripcion { get; set; }

        [Column("usuario")]
        [Required]
        public string usuario { get; set; }

        
    }
}