using ProyectoApiContable.Dtos.FilasPartidas;

namespace ProyectoApiContable.Dtos.Partidas;

public class PartidaDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string CreadoPor { get; set; }
    public int EstadoPartidaId { get; set; }
    public string RevisadoPor { get; set; }
    public DateTime FechaRevision { get; set; }
    public List<FilasPartidaDto> FilasPartida { get; set; }

}