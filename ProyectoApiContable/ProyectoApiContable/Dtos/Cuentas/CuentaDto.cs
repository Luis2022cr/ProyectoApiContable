namespace ProyectoApiContable.Dtos.Catalogos
{
    public class CuentaDto
    {
      
        public Guid Id { get; set; }
        
        public string Nombre { get; set; }
        
        public int Codigo { get; set; }
        
        public int TipoCuentaId { get; set; }
        
        public decimal Saldo { get; set; }
        
        public string Descripcion { get; set; }
        
        public DateTime FechaCreacion { get; set; }
        
        public string FechaCreacionFormatted => FechaCreacion.ToString("dd/MM/yyyy");
        
    }
       
    
}
