﻿using System.ComponentModel.DataAnnotations;

namespace ProyectoApiContable.Dtos.Usuarios
{
    public class RegisterUserDto
    {
        [Display(Name = "Correo Electronico")]
        [Required(ErrorMessage = "El {0} es requerido.")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", 
            ErrorMessage = "Ingrese un {0} valido.")]

        public string Email { get; set; }

        [Display(Name = "rol")]
        [Required(ErrorMessage = "la {0} es requerida.")]
        [RegularExpression("^(Admin|Empleado)$", ErrorMessage = "El {0} solo puede ser 'Admin' o 'Empleado'.")]
        public string Rol { get; set; }

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "la {0} es requerida.")]
        public string Password { get; set; }
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; }
    }
}
