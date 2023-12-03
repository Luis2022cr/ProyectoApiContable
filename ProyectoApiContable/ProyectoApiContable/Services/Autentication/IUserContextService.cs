using Microsoft.AspNetCore.Identity;

namespace ProyectoApiContable.Services.Autentication;

public interface IUserContextService
{
   Task<IdentityUser> GetUserAsync();
}