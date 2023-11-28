using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProyectoApiContable.Entities;

[Table("partidas")]

public class Partida
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("nombre")]
    [StringLength(50)]
    [Required]
    public string Nombre { get; set; }

    [Column("descripcion")]
    public string Descripcion { get; set; }
        
    [Column("fecha_creacion")]
    [Required]
    [DataType(DataType.Date)]
    public DateTime FechaCreacion { get; set; }
    
    [Column("usuario_id")]
    [Required]
    public string UsuarioId { get; set; }
    
    [Column("aprobado")]
    [Required]
    public bool Aprobado { get; set; }
    
    [ForeignKey(nameof(UsuarioId))] public virtual IdentityUser User { get; set; }
    
    
    public virtual ICollection<FilasPartida> FilasPartida { get; set; }
    
}