namespace BuildingBlocks.Source.Infrastructure.Security;

public interface IJwtTokenService
{
    /// <summary>
    /// Genera un token JWT para el usuario autenticado.
    /// </summary>
    /// <param name="userId">Identificador del usuario.</param>
    /// <param name="cedula">Cédula del usuario.</param>
    /// <param name="nombre">Nombre completo del usuario.</param>
    /// <param name="rol">Rol del usuario (DIRECTOR, DOCENTE).</param>
    /// <returns>Token JWT y fecha de expiración.</returns>
    (string Token, DateTime ExpiresAt) GenerateToken(long userId, string cedula, string nombre, string rol);
}
