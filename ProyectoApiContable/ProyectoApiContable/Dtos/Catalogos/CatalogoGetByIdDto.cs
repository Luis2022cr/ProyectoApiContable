using ProyectoApiContable.Entities;

namespace ProyectoApiContable.Dtos.Catalogos
{
    public class CatalogoGetByIdDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Usuario { get; set; }
        public string Description { get; set; }
        public DateTime Fecha { get; set; }

    }
}
