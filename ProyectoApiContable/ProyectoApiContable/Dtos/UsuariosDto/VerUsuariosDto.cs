namespace ProyectoApiContable.Dtos.UsuariosDto
{
    public class VerUsuariosDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public List<string> Rol { get; set; }
    }
}
