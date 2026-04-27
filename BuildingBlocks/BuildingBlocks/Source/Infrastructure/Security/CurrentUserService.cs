#region

using System.Security.Claims;
using Microsoft.AspNetCore.Http;

#endregion

namespace BuildingBlocks.Source.Infrastructure.Security;

public class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    public long? UserId
    {
        get
        {
            var v = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(v, out var id) ? id : null;
        }
    }

    public string? Cedula => accessor.HttpContext?.User.FindFirst("cedula")?.Value;
    public string? Nombre => accessor.HttpContext?.User.FindFirst("nombre")?.Value;
    public string? Rol => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);

    public bool IsAuthenticated => accessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    public bool IsDirector => string.Equals(Rol, "DIRECTOR", StringComparison.OrdinalIgnoreCase);
    public bool IsDocente => string.Equals(Rol, "DOCENTE", StringComparison.OrdinalIgnoreCase);
}
