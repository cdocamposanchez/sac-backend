#region

using BuildingBlocks.Source.Application.CQRS;
using BuildingBlocks.Source.Domain.Exception;
using BuildingBlocks.Source.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Sac.Application.Persistance;
using Sac.Domain.Dtos;
using Sac.Domain.Enums;

#endregion

namespace Sac.Application.Modules.Auth.Commands.Login;

internal sealed class LoginCommandHandler(
    IApplicationDbContext db,
    IJwtTokenService jwt) : ICommandHandler<LoginCommand, LoginResponseDto>
{
    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var cedula = request.Credentials.Cedula.Trim();
        var password = request.Credentials.Password;

        // 1) Director (RF-01: redirección por rol)
        var director = await db.Directores
            .FirstOrDefaultAsync(d => d.Cedula == cedula, cancellationToken);

        if (director != null)
        {
            if (!director.Activo)
                throw new UnauthorizedException("Su cuenta de Director se encuentra desactivada.");

            if (!director.VerificarPassword(password))
                throw new UnauthorizedException("Cédula o contraseña incorrecta.");

            var (tokenD, expD) = jwt.GenerateToken(
                director.Id, director.Cedula, director.NombreCompleto, RolUsuario.Director);

            return new LoginResponseDto(
                director.Id, director.Cedula, director.NombreCompleto, director.Correo,
                RolUsuario.Director, tokenD, expD);
        }

        // 2) Docente
        var docente = await db.Docentes
            .FirstOrDefaultAsync(d => d.Cedula == cedula, cancellationToken);

        if (docente == null)
            throw new UnauthorizedException("Cédula o contraseña incorrecta.");

        if (!docente.Activo)
            throw new UnauthorizedException("Su cuenta de Docente se encuentra desactivada.");

        if (!docente.VerificarPassword(password))
            throw new UnauthorizedException("Cédula o contraseña incorrecta.");

        var (token, exp) = jwt.GenerateToken(
            docente.Id, docente.Cedula, docente.NombreCompleto, RolUsuario.Docente);

        return new LoginResponseDto(
            docente.Id, docente.Cedula, docente.NombreCompleto, docente.Correo,
            RolUsuario.Docente, token, exp);
    }
}
