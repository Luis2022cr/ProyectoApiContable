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

    [Column("descripcion")] public string Descripcion { get; set; }

    [Column("fecha_creacion")]
    [Required]
    [DataType(DataType.Date)]
    public DateTime FechaCreacion { get; set; }
    
    [Column("fecha_aprobacion")]
    [DataType(DataType.Date)]
    public DateTime? FechaRevision { get; set; }

    [Column("creado_por")] [Required] public string CreadoPorId { get; set; }

    [Column("estado_partida_id")]
    [Required]
    public int EstadoPartidaId { get; set; }

    [Column("revisado_por")]  public string? RevisadoPorId { get; set; }

    [ForeignKey(nameof(CreadoPorId))] public virtual IdentityUser CreadoPor { get; set; }

    [ForeignKey(nameof(RevisadoPorId))] public virtual IdentityUser RevisadoPor { get; set; }


    [ForeignKey(nameof(EstadoPartidaId))] public virtual EstadoPartida EstadoPartida { get; set; }
    public virtual List<FilasPartida> FilasPartida { get; set; }
}