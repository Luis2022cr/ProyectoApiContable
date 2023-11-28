using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace ProyectoApiContable.Entities;

 
    [Table("cuentas")]
    public class Cuenta
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("nombre")] 
        [StringLength(50)]
        [Required]
        public string Nombre { get; set; }

        [Column("codigo")]
        [StringLength(50)]
        [Required]
        public int Codigo { get; set; }

        [Column("descripcion")]
        [StringLength(255)]
        public string Descripcion { get; set; }

        [Column("fechaCreacion")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime FechaCreacion { get; set; }
        
        public ICollection<FilasPartida> FilasPartida { get; set; }
        
    }
    
