using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProyectoApiContable.Dtos;
using ProyectoApiContable.Dtos.Usuarios;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProyectoApiContable.Controllers
{
    [Route("api/usuarios")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public UsuariosController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("Login")]
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
                    new Claim("UserId", user.Id)
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
                    FullName = "",
                    TokenExpiration = jwtToken.ValidTo,
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtToken)
                };

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

        [HttpPost("Register")]
        public async Task<ActionResult<ResponseDto<Object>>> RegisterUser(
            RegisterUserDto dto)
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

            // Cada usuario le asignaremos el rol de User
            await _userManager.AddToRoleAsync(indetityUser, "User");

            return Ok(new ResponseDto<object>
            {
                Status = true,
                Data = dto
            }); ;
        }

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
