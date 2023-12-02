using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProyectoApiContable.Entities;

[Table("estados_partidas")]
public class EstadoPartida
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("nombre")]  
    [StringLength(50)]
    [Required]
    public string Nombre { get; set; }
    
    public ICollection<Partida> Partidas { get; set; }
}