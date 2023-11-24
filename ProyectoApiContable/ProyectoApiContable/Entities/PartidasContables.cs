using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoApiContable.Entities
{
    [Table("partidas_contables", Schema = "contable")]
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

        [Column("fecha")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Column("cuenta_debe_id")]
        [Required]
        public int CuentaDebeId { get; set; }

        [Column("cuenta_haber_id")]
        [Required]
        public int CuentaHaberId { get; set; }

        [Column("monto")]
        [Required]
        public decimal Monto { get; set; }

        [Column("descripcion")]
        [StringLength(255)]
        public string Descripcion { get; set; }

        [Column("usuario")]
        [Required]
        public string usuario { get; set; }

        public List<PartidasCuentas> PartidasCuentas { get; set; }
    }
}