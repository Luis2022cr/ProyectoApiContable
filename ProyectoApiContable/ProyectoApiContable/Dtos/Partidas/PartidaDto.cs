namespace ProyectoApiContable.Dtos.Partidas;

public class PartidaDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string FechaCreacionFormatted => FechaCreacion.ToString("dd/MM/yyyy");
    public string UsuarioId { get; set; }
    public bool Aprobado { get; set; } 
}