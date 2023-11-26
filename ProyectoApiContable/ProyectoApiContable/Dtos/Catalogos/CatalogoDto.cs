namespace ProyectoApiContable.Dtos.Catalogos
{
    public class CatalogoDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Usuario { get; set; }

        public int Total { get; set;}

        public string Description { get; set; }

        public DateTime Fecha { get; set; }
    }
}
