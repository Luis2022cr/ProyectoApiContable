using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


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

        [Column("tipo_cuenta_id")]
        public int TipoCuentaId { get; set; }
        
        [Column("codigo")]
        [StringLength(50)]
        [Required]
        public int Codigo { get; set; }

        [Column("descripcion")]
        [StringLength(255)]
        public string Descripcion { get; set; }

        [Column("fecha_creacion")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime FechaCreacion { get; set; }
        
        [Column("saldo")]
        public decimal Saldo { get; set; }

        [ForeignKey("TipoCuentaId")]
        public TipoCuenta TipoCuenta { get; set; }
        public ICollection<FilasPartida> FilasPartida { get; set; }
        
    }
    
