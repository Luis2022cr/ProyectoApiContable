using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoApiContable.Entities
{
    [Table("filas_partidas")]
    public class FilasPartida
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("debito")]
        public decimal Debito { get; set; }

        [Column("credito")]
        public decimal Credito { get; set; }

        [Column("cuenta_id")]
        [Required]
        public Guid CuentaId { get; set; }
        
        [Column("partida_id")]
        [Required]
        public Guid PartidaId { get; set; }
        
        [ForeignKey(nameof(CuentaId))] public virtual Cuenta Cuenta { get; set; }
        [ForeignKey(nameof(PartidaId))] public virtual Partida Partida { get; set; }
        
    }
}