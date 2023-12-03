using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ProyectoApiContable.Services.Autentication;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<IdentityUser> _userManager;


    public UserContextService(IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }


    public async Task<IdentityUser> GetUserAsync()
    {
        var usuarioActual = _httpContextAccessor.HttpContext.User;
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext?.User?.Identity?.IsAuthenticated ?? false)
        {
            var userId = usuarioActual.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var usuario = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
            
            if (usuario == null)
            {
                throw new Exception("El usuario no existe");
            }
            // Usuario autenticado, devolver el principal de claims
            return usuario;
        }

        // No autenticado, puedes manejar esto según tus necesidades (por ejemplo, redirigir a la página de inicio de sesión)
        return null;
    }
}