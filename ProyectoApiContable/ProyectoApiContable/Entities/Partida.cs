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
    
    [Column("creado_por_id")]
    [Required]
    public string CreadoPorId { get; set; }
    
    [Column("estado_partida_id")]
    [Required]
    public int EstadoPartidaId { get; set; }
    
    [Column("aprobado_por_id")]
    [Required]
    public string AprobadoPorId { get; set; }
    
    [ForeignKey(nameof(CreadoPorId))]
    public virtual IdentityUser CreadoPor { get; set; }

    [ForeignKey(nameof(AprobadoPorId))]
    public virtual IdentityUser AprobadoPor { get; set; }
    
    
    [ForeignKey(nameof(EstadoPartidaId))]
    public virtual EstadoPartida EstadoPartida { get; set; }
    public virtual ICollection<FilasPartida> FilasPartida { get; set; }
    
}