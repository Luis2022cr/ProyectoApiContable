namespace ProyectoApiContable.Dtos.UsuariosDto
{
    public class ResetContrasenaDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
