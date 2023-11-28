namespace ProyectoApiContable.Dtos.FilasPartidas;

public class FilasPartidaDto
{
    public Guid Id { get; set; }
    public decimal Debito { get; set; }
    public decimal Credito { get; set; }
    public Guid CuentaId { get; set; }
    public Guid PartidaId { get; set; }
}