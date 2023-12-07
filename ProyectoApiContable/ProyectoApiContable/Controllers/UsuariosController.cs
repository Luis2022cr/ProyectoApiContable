using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProyectoApiContable.Dtos;
using ProyectoApiContable.Dtos.Usuarios;
using ProyectoApiContable.Dtos.UsuariosDto;
using ProyectoApiContable.Helpers;
using ProyectoApiContable.Services;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace ProyectoApiContable.Controllers
{
    [Route("api/usuarios")]
    [ApiController]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IRedisServices _redisServices;

        public UsuariosController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            IEmailService emailService,
            IRedisServices redisServices
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
            _redisServices = redisServices;
        }

        [HttpGet("Usuarios")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseDto<IEnumerable<VerUsuariosDto>>>> ObtenerTodosLosUsuarios()
        {
            var users = await _userManager.Users.ToListAsync();

            if (users.Count == 0)
            {
                return NotFound(new ResponseDto<IEnumerable<VerUsuariosDto>>
                {
                    Status = false,
                    Message = "No se encontraron usuarios"
                });
            }

            var usersDto = new List<VerUsuariosDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                var userDto = new VerUsuariosDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    Rol = roles.ToList()
                };

                usersDto.Add(userDto);
            }

            return Ok(new ResponseDto<IEnumerable<VerUsuariosDto>>
            {
                Status = true,
                Data = usersDto
            });
        }


        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDto<LoginResponseDto>>> Login(LoginDto dto)
        {
            var result = await _signInManager
                .PasswordSignInAsync(dto.Email, dto.Password,
                    isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);

                // Crear Claims
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new(ClaimTypes.NameIdentifier, user.Id),
                    new(ClaimTypes.Name, $"{user.UserName}"),
                };

                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                // Generar token
                var jwtToken = GetToken(authClaims);

                var loginResponseDto = new LoginResponseDto
                {
                    Email = user.Email,
                    Name = user.UserName,
                    TokenExpiration = jwtToken.ValidTo,
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtToken)
                };

                // Obtener la fecha actual
                var fechaActual = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss");

                //Agregar el log en redis
                await _redisServices.AgregarLogARedis($"El usuario: {user.Email} inicio sesion - [{fechaActual}]");

                await _emailService.SendEmailAsync(user.Email, "ApiContable -- Inicio de sesion",
                    EmailTemplates.LoginTemplate(user.Email));

                return Ok(new ResponseDto<LoginResponseDto>
                {
                    Status = true,
                    Message = "Autenticacion satisfactoria",
                    Data = loginResponseDto
                });

            }
            return StatusCode(StatusCodes.Status401Unauthorized, new ResponseDto<LoginResponseDto>
            {
                Status = false,
                Message = "La autenticacion fallo."
            });
        }

        [HttpPost("Registro")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseDto<object>>> RegistroUser(RegisterUserDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is not null)
            {
                return BadRequest(new ResponseDto<object>
                {
                    Status = false,
                    Message = $"El usuario con el correo {dto.Email} ya se encuentra registrado"
                });
            }

            // Validar que el rol sea 'Admin' o 'User'
            if (dto.Rol != "Admin" && dto.Rol != "Empleado")
            {
                return BadRequest(new ResponseDto<object>
                {
                    Status = false,
                    Message = $"El rol debe ser 'Admin' o 'Empleado'."
                });
            }

            var indetityUser = new IdentityUser
            {
                Email = dto.Email,
                UserName = dto.Email
            };

            var result = await _userManager.CreateAsync(indetityUser, dto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new ResponseDto<object>
                {
                    Status = false,
                    Message = $"Fallo el registro del usuario",
                    Data = result.Errors
                });
            }

            // Asignar rol al usuario registrado
            await _userManager.AddToRoleAsync(indetityUser, dto.Rol);

            // Obtener la fecha y hora actual
            var fechaActual = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss");

            //Obtener el email del registrador
            var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            //Agregar el log en redis
            await _redisServices.AgregarLogARedis($"El usuario {userEmail} a registrado un nuevo usuario: {dto.Email} con Rol: {dto.Rol}- [{fechaActual}]");

            //Enviar correo de Registro
            await _emailService.SendEmailAsync(dto.Email, "ApiContable -- Registro de cuenta",
                    EmailTemplates.RegistroTemplate(dto.Email));

            return Ok(new ResponseDto<object>
            {
                Status = true,
                Data = dto
            }); ;
        }

        [HttpPut("CambiarRol/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseDto<object>>> CambiarRolUsuario(string Id, CambiarRolUsuario dto)
        {
            // Buscar al usuario por su ID
            var user = await _userManager.FindByIdAsync(Id);

            if (user is null)
            {
                return NotFound(new ResponseDto<object>
                {
                    Status = false,
                    Message = $"Usuario con ID {Id} no encontrado"
                });
            }

            // Validar que el rol sea 'Admin' o 'Empleado'
            if (dto.Rol != "Admin" && dto.Rol != "Empleado")
            {
                return BadRequest(new ResponseDto<object>
                {
                    Status = false,
                    Message = $"El rol debe ser 'Admin' o 'Empleado'."
                });
            }

            // Obtener los roles existentes del usuario
            var userRoles = await _userManager.GetRolesAsync(user);

            // Remover todos los roles actuales del usuario
            await _userManager.RemoveFromRolesAsync(user, userRoles);

            // Asignar el nuevo rol al usuario
            await _userManager.AddToRoleAsync(user, dto.Rol);

            // Obtener la fecha y hora actual
            var fechaActual = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss");

            // Obtener el email del usuario autenticado
            var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            // Agregar el log en redis
            await _redisServices.AgregarLogARedis($"El usuario {userEmail} ha cambiado el rol del usuario con ID ' {Id} ' a: {dto.Rol} - [{fechaActual}]");

            return Ok(new ResponseDto<object>
            {
                Status = true,
                Data = dto
            });
        }

        [HttpPost("RecuperarContrasena")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDto<object>>> RecuperarContrasena(RecuperarDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var tokenCodificado = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            //Enviar correo
            await _emailService.SendEmailAsync(user.Email, "ApiContable -- Recuperacion de contraseña",
                EmailTemplates.RecuperarContrasenaTemplate(tokenCodificado));

            // Obtener la fecha y hora actual
            var fechaActual = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss");

            //Agregar el log en redis
            await _redisServices.AgregarLogARedis($"Se solicito recuperaion de contraseña: {dto.Email} - [{fechaActual}]");

            return Ok(new ResponseDto<object>
            {
                Status = true,
                Message = "Revisar el correo electrónico para restablecer la contraseña."
            });
        }

        [HttpPost("ResetContrasena")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDto<object>>> ResetContrasena(ResetContrasenaDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new ResponseDto<object>
                {
                    Status = false,
                    Message = "Usuario inválido"
                });
            }

            var decodedTokenBytes = WebEncoders.Base64UrlDecode(model.Token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
            if (result.Succeeded)
            {
                // Obtener la fecha y hora actual
                var fechaActual = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss");

                //Agregar el log en redis
                await _redisServices.AgregarLogARedis($"Se cambio la contraseña de: {user.Email} - [{fechaActual}]");

                await _emailService.SendEmailAsync(user.Email, "ApiContable -- Contraseña Restablecida",
                    EmailTemplates.ContrasenaRestablecidaTemplate(user.Email));

                return Ok(new ResponseDto<object>
                {
                    Status = true,
                    Message = "La contraseña se restableció correctamente."
                });
            }

            return BadRequest(new ResponseDto<object>
            {
                Status = false,
                Message = "No se pudo restablecer la contraseña.",
                Data = result.Errors
            });
        }

        [HttpPost("CambiarContrasenaAdmin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CambiarContrasenaAdmin([FromBody] RecuperarPasswordAdminDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new { Message = "Usuario no encontrado." });
            }

            var result = await _userManager.RemovePasswordAsync(user);
            if (result.Succeeded)
            {
                result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (result.Succeeded)
                {
                    // Obtener la fecha y hora actual
                    var fechaActual = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss");

                    //Agregar el log en redis
                    await _redisServices.AgregarLogARedis($"un Admin cambio la contraseña de: {user.Email} - [{fechaActual}]");

                    await _emailService.SendEmailAsync(user.Email, "ApiContable -- Contraseña Restablecida",
                        EmailTemplates.ContrasenaRestablecidaAdminTemplate(user.Email));

                    return Ok(new ResponseDto<object>
                    {
                        Status = true,
                        Message = "Contraseña cambiada correctamente."
                    });
                }
            }

            return BadRequest(new ResponseDto<object>
            {
                Status = false,
                Message = "No se pudo restablecer la contraseña."
            });
        }

        //Obtener token
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSinginKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSinginKey,
                    SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

    }
}
