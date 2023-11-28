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
        
        
        public ICollection<PartidasContables> PartidasContablesDebito { get; set; }
        public ICollection<PartidasContables> PartidasContablesCredito { get; set; }

       
    }
    
