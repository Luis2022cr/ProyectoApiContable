﻿namespace ProyectoApiContable.Dtos.Usuarios
{
    public class LoginResponseDto
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpiration { get; set; }
    }
}
