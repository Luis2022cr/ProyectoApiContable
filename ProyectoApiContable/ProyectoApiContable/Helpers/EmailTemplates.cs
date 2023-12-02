namespace ProyectoApiContable.Helpers
{
    public static class EmailTemplates
    {
        public static string LoginTemplate(string email)
        {
            return $@"
            <h3>Inicio de sesion realizado correctamente.</h3>
            <p>Le informamos que se ha iniciado sesion en su cuenta de ApiContable</p>         
            <p>Fecha y hora de inicio de sesion: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}</p>
            ";
        }

        public static string RegistroTemplate(string email)
        {
            return $@"
            <h3>Registro en ApiContable realizado correctamente.</h3>
            <p>Informamos que se ha creado una cuenta de ApiContable con su correo electronico</p>         
            <p>Fecha y hora de creacion: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}</p>
            ";
        }

        public static string RecuperarContrasenaTemplate(string token)
        {
            return $@"
            <h3>Recuperación de contraseña</h3>
            <p>Hemos recibido una solicitud para restablecer la contraseña de su cuenta en ApiContable.</p>
            <p> token de recuperacion = ' {token} '</p>
            <p>Si no solicitó un restablecimiento de contraseña, ignore este mensaje.</p>
            <p>Este token expirará en 1 hora por motivos de seguridad.</p>
            ";
        }

        public static string ContrasenaRestablecidaTemplate(string email)
        {
            return $@"
            <h3>Recuperación de contraseña</h3>
            <p>Se ha cambiado la contraseña de su cuenta en ApiContable correctamente.</p>
            <br/>
            <p>Si no restablecio su contraseña, notifique al administrador lo antes posible</p>
            ";
        }

        public static string ContrasenaRestablecidaAdminTemplate(string email)
        {
            return $@"
            <h3>Recuperación de contraseña</h3>
            <p>Se ha cambiado la contraseña de su cuenta en ApiContable correctamente por un Administrador.</p>
            <br/>
            <p>Si no restablecio su contraseña, notifique a un administrador lo antes posible</p>
            ";
        }
    }
}
