namespace BuildingBlocks.Source.Infrastructure.Security;

public interface ICurrentUserService
{
    long? UserId { get; }
    string? Cedula { get; }
    string? Nombre { get; }
    string? Rol { get; }
    bool IsAuthenticated { get; }
    bool IsDirector { get; }
    bool IsDocente { get; }
}
