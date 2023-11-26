using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProyectoApiContable.Entities;

 
    [Table("catalogo_cuentas")]
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

        [Column("descripcion")]
        [StringLength(255)]
        public string Descripcion { get; set; }

        [Column("fechaCreacion")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime FechaCreacion { get; set; }

        [NotMapped]
        public decimal Total => CalcularTotalPartidas();
        
        public ICollection<PartidasContables> PartidasContablesDebito { get; set; }
        public ICollection<PartidasContables> PartidasContablesCredito { get; set; }

        public decimal CalcularTotalPartidas()
        {
            decimal totalDebito = PartidasContablesDebito?.Sum(p => p.MontoDebito) ?? 0;
            decimal totalCredito = PartidasContablesCredito?.Sum(p => p.MontoCredito) ?? 0;

            return totalDebito - totalCredito;
        }
    }
    
